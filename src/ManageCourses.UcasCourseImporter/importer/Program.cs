using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using CsvHelper;
using GovUk.Education.ManageCourses.Api;
using GovUk.Education.ManageCourses.Csv.Domain;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Xls;
using GovUk.Education.ManageCourses.Xls.Domain;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace GovUk.Education.ManageCourses.UcasCourseImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = GetConfiguration();
            var telemetryClient = new TelemetryClient() { InstrumentationKey = configuration["APPINSIGHTS_INSTRUMENTATIONKEY"] };

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo
                .ApplicationInsightsTraces(configuration["APPINSIGHTS_INSTRUMENTATIONKEY"])
                .CreateLogger();

            var folder = Path.Combine(Path.GetTempPath(), "ucasfiles", Guid.NewGuid().ToString());
            try
            {
                logger.Information("UcasCourseImporter started.");

                var configOptions = new UcasCourseImporterConfigurationOptions(configuration);
                configOptions.Validate();
                var mcConfig = new McConfig(configuration);
                mcConfig.Validate();


                Directory.CreateDirectory(folder);

                var downloadAndExtractor = new DownloaderAndExtractor(logger, folder, configOptions.AzureUrl,
                    configOptions.AzureSignature);

                var unzipFolder = downloadAndExtractor.DownloadAndExtractLatest("NetupdateExtract");
                var unzipFolderProfiles = downloadAndExtractor.DownloadAndExtractLatest("EntryProfilesExtract_test");

                var xlsReader = new XlsReader(logger);

                // only used to avoid importing orphaned data
                // i.e. we do not import institutions but need them to determine which campuses to import
                var subjects = xlsReader.ReadSubjects("data");

                // entry profile data - used to correct institution data
                var institutionProfiles = ReadInstitutionProfiles(unzipFolderProfiles);

                // data to import
                var institutions = xlsReader.ReadInstitutions(unzipFolder);
                UpdateContactDetails(institutions, institutionProfiles);
                var campuses = xlsReader.ReadCampuses(unzipFolder, institutions);
                var courses = xlsReader.ReadCourses(unzipFolder, campuses);
                var courseSubjects = xlsReader.ReadCourseSubjects(unzipFolder, courses, subjects);
                var courseNotes = xlsReader.ReadCourseNotes(unzipFolder);
                var noteTexts = xlsReader.ReadNoteText(unzipFolder);

                var payload = new UcasPayload
                {
                    Institutions = new List<Xls.Domain.UcasInstitution>(institutions),
                    Courses = new List<UcasCourse>(courses),
                    CourseSubjects = new List<UcasCourseSubject>(courseSubjects),
                    Campuses = new List<UcasCampus>(campuses),
                    CourseNotes = new List<UcasCourseNote>(courseNotes),
                    NoteTexts = new List<UcasNoteText>(noteTexts),
                    Subjects = new List<UcasSubject>(subjects)
                };
                var context = GetDbContext(mcConfig);
                var ucasDataMigrator = new UcasDataMigrator(context, logger, payload);
                ucasDataMigrator.UpdateUcasData();
            }
            catch (Exception e)
            {
                logger.Error(e, "UcasCourseImporter error.");
            }
            finally
            {
                CleanupTempData(folder, logger);
                logger.Information("UcasCourseImporter finished.");

                // flush logs and wait for them to be written. https://github.com/serilog/serilog-sinks-applicationinsights#how-when-and-why-to-flush-messages-manually
                telemetryClient.Flush();
                Thread.Sleep(5000);
            }
        }

        private static ManageCoursesDbContext GetDbContext(McConfig config)
        {
            var connectionString = config.BuildConnectionString();

            var options = new DbContextOptionsBuilder<ManageCoursesDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            return new ManageCoursesDbContext(options);
        }

        private static void CleanupTempData(string folder, ILogger logger)
        {
            try
            {
                var di = new DirectoryInfo(folder);
                di.Delete(true);
            }
            catch (Exception e)
            {
                logger.Error(e, string.Format(CultureInfo.CurrentCulture, "CleanupTempData({0}) failed.", folder));
            }
        }
        private static Dictionary<string, UcasInstitutionProfile> ReadInstitutionProfiles(string unzipFolderProfiles)
        {
            var institutionProfiles = new Dictionary<string, UcasInstitutionProfile>();

            using (var fileStr = File.OpenText(Path.Combine(unzipFolderProfiles, "gttr_inst.csv")))
            {
                var institutionProfilesCsv = new CsvReader(fileStr);
                institutionProfilesCsv.Read();
                institutionProfilesCsv.ReadHeader();
                while (institutionProfilesCsv.Read())
                {
                    var rec = institutionProfilesCsv.GetRecord<UcasInstitutionProfile>();
                    institutionProfiles[rec.inst_code] = rec;
                }
            }

            return institutionProfiles;
        }

        private static void UpdateContactDetails(List<Xls.Domain.UcasInstitution> institutions, IDictionary<string, UcasInstitutionProfile> institutionProfiles)
        {
            foreach (var inst in institutions)
            {
                if (institutionProfiles.TryGetValue(inst.InstCode, out UcasInstitutionProfile profile))
                {
                    inst.Addr1 = profile.inst_address1.Trim();
                    inst.Addr2 = profile.inst_address2.Trim();
                    inst.Addr3 = profile.inst_address3.Trim();
                    inst.Addr4 = profile.inst_address4.Trim();
                    inst.Postcode = profile.inst_post_code.Trim();
                    inst.ContactName = profile.inst_person.Trim();
                    inst.Email = profile.email.Trim();
                    inst.Telephone = profile.inst_tel.Trim();
                    inst.Url = profile.web_addr.Trim();
                    inst.RegionCode = profile.region_code;
                }
            }
        }

        private static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
