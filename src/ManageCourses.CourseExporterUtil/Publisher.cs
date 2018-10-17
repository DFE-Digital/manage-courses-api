using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.SearchAndCompare.Domain.Client;
using GovUk.Education.SearchAndCompare.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SearchCourse = GovUk.Education.SearchAndCompare.Domain.Models.Course;

namespace GovUk.Education.ManageCourses.CourseExporterUtil
{
    /// <summary>
    /// Pulls data out of manage database and pushes it in bulk into search api.
    /// </summary>
    public class Publisher
    {
        private McConfig _mcConfig;

        /// <summary>
        /// Pull data out of manage database and push it in bulk into search api.
        /// </summary>
        public void PublishToSearch()
        {
            _mcConfig = GetConfig();
            var context = GetContext();
            var mappedCourses = ReadAllCourseData(context);
            PublishToSearch(mappedCourses).Wait();
            Console.WriteLine("Done");
        }

        private async Task PublishToSearch(IList<SearchCourse> courses)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mcConfig.SearchAndCompareApiKey);
                var client = new SearchAndCompareApi(httpClient, _mcConfig.SearchAndCompareApiUrl);
                Console.WriteLine($"Sending {courses.Count()} courses to Search API...");
                var success = await client.SaveCoursesAsync(courses);
                if (!success)
                {
                    throw new Exception("Publishing to Search API failed. Check search API logs for details");
                }
            }
        }

        private static List<SearchCourse> ReadAllCourseData(IManageCoursesDbContext context)
        {
            Console.WriteLine("Retrieve courses");
            var ucasCourses = context.UcasCourses.Include(x => x.UcasInstitution)
                .Include(x => x.UcasInstitution.UcasCourseSubjects).ThenInclude(x => x.UcasSubject)
                .Include(x => x.CourseCode).ThenInclude(x => x.UcasCourseSubjects)
                .Include(x => x.AccreditingProviderInstitution)
                .Include(x => x.UcasCampus)
                .Where(x => x.Publish == "Y")
                .ToList();
            Console.WriteLine($" - {ucasCourses.Count()} courses");

            Console.WriteLine("Retrieve institutions");
            var insts = context.UcasInstitutions.ToDictionary(x => x.InstCode);
            Console.WriteLine($" - {insts.Count()} institutions");

            Console.WriteLine("Retrieve enrichments");
            var courseEnrichments = context.CourseEnrichments
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .Where(x => x.Status == EnumStatus.Published)
                .ToLookup(x => x.InstCode + "_@@_" + x.UcasCourseCode)
                .ToDictionary(x => x.Key, x => x.OrderByDescending(y => y.UpdatedTimestampUtc).First());
            Console.WriteLine($" - {courseEnrichments.Count()} courseEnrichments");

            var orgEnrichments = context.InstitutionEnrichments
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .Where(x => x.Status == EnumStatus.Published)
                .ToLookup(x => x.InstCode)
                .ToDictionary(x => x.Key, x => x.OrderByDescending(y => y.UpdatedTimestampUtc).First());
            Console.WriteLine($" - {orgEnrichments.Count()} orgEnrichments");

            var pgdeCourses = context.PgdeCourses.ToList();
            Console.WriteLine($" - {pgdeCourses.Count()} pgdeCourses");

            Console.WriteLine("Load courses");
            var courses = new CourseLoader().LoadCourses(ucasCourses, new List<UcasCourseEnrichmentGetModel>(), pgdeCourses);

            var courseMapper = new CourseMapper();
            var converter = new EnrichmentConverter();

            var mappedCourses = new List<SearchAndCompare.Domain.Models.Course>();

            Console.WriteLine("Combine courses with institution and enrichment data");

            foreach (var c in courses.Courses)
            {
                var mappedCourse = courseMapper.MapToSearchAndCompareCourse(
                    insts[c.InstCode],
                    c,
                    converter.Convert(orgEnrichments.GetValueOrDefault(c.InstCode))?.EnrichmentModel,
                    converter.Convert(courseEnrichments.GetValueOrDefault(c.InstCode + "_@@_" + c.CourseCode))?.EnrichmentModel
                );

                if (!mappedCourse.Campuses.Any())
                {
                    // only publish running courses
                    continue;
                }

                if (!mappedCourse.CourseSubjects.Any())
                {
                    Console.WriteLine(
                        $"failed to assign subject to [{c.InstCode}]/[{c.CourseCode}] {c.Name}. UCAS tags: {c.Subjects}");
                    // only publish courses we could map to one or more subjects.
                    continue;
                }

                // hacks - remove when coursemapper refactor has completed
                mappedCourse.ProviderLocation = new Location { Address = mappedCourse.ContactDetails.Address };
                mappedCourse.Fees = mappedCourse.Fees ?? new Fees();
                mappedCourse.Salary = mappedCourse.Salary ?? new Salary();

                mappedCourses.Add(mappedCourse);
            }

            return mappedCourses;
        }

        private ManageCoursesDbContext GetContext()
        {
            var connectionString = _mcConfig.BuildConnectionString();

            var options = new DbContextOptionsBuilder<ManageCoursesDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            return new ManageCoursesDbContext(options);
        }

        private static McConfig GetConfig()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                    optional: true)
                .AddUserSecrets<Api.Startup>()
                .AddEnvironmentVariables()
                .Build();

            var mcConfig = new McConfig(config);
            mcConfig.Validate();
            return mcConfig;
        }
    }
}
