using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Serilog;
using Serilog.Core;
using SearchCourse = GovUk.Education.SearchAndCompare.Domain.Models.Course;

namespace GovUk.Education.ManageCourses.CourseExporterUtil
{
    /// <summary>
    /// Pulls data out of manage database and pushes it in bulk into search api.
    /// </summary>
    public class Publisher
    {
        private readonly McConfig _mcConfig;
        private readonly Logger _logger;

        public Publisher(IConfiguration configuration)
        {
            _logger = GetLogger(configuration);
            _mcConfig = GetMcConfig(configuration);
        }

        /// <summary>
        /// Pull data out of manage database and push it in bulk into search api.
        /// </summary>
        public void Publish()
        {
            _logger.Information("Bulk publish to search started");
            var context = GetContext();
            var mappedCourses = ReadAllCourseData(context);
            try
            {

                PublishToSearch(mappedCourses).Wait();
                _logger.Information("Bulk publish to search completed");
            }
            catch (Exception ex)
            {
                // Still exit cleanly even if the POST failed. The post takes longer than the timeout on the gateway
                // so we usually get a 502 instead of 200. We have monitoring & alerts in azure so that we will notice
                // if the receiving end isn't completing its side.
                if (ex.Message.Contains("timeout"))
                {
                    _logger.Information($"{nameof(PublishToSearch)} threw, this likely the usual gateway timeout. {ex.Message}");
                    return;
                }
                _logger.Error($"{nameof(PublishToSearch)} threw, but didn't seem to be a timeout.", ex);
            }
        }

        private async Task PublishToSearch(IList<SearchCourse> courses)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mcConfig.SearchAndCompareApiKey);
                httpClient.Timeout = TimeSpan.FromMinutes(120);
                var client = new SearchAndCompareApi(httpClient, _mcConfig.SearchAndCompareApiUrl);
                _logger.Information($"Sending {courses.Count()} courses to Search API...");
                var success = await client.SaveCoursesAsync(courses);
                if (!success)
                {
                    throw new Exception("Publishing to Search API failed. Check search API logs for details");
                }
            }
        }

        public List<SearchCourse> ReadAllCourseData(IManageCoursesDbContext context)
        {
            _logger.Information("Retrieving courses");
            var courses = context.Courses
                .Where(x => x.Provider.RecruitmentCycle.Year == RecruitmentCycle.CurrentYear)
                .Include(x => x.Provider)
                .Include(x => x.CourseSubjects).ThenInclude(x => x.Subject)
                .Include(x => x.AccreditingProvider)
                .Include(x => x.CourseSites).ThenInclude(x => x.Site)
                .Include(x => x.CourseEnrichments).ThenInclude(x => x.CreatedByUser)
                .Include(x => x.CourseEnrichments).ThenInclude(x => x.UpdatedByUser)
                .ToList();

            foreach(var course in courses)
            {
                course.CourseSites = new Collection<CourseSite>(course.CourseSites.Where(x => x.Publish == "Y").ToList());
            }

            _logger.Information($" - {courses.Count()} courses");

            _logger.Information("Retrieving enrichments");

            var orgEnrichments = context.ProviderEnrichments
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .Where(x => x.Status == EnumStatus.Published && x.Provider.RecruitmentCycle.Year == RecruitmentCycle.CurrentYear)
                .ToLookup(x => x.ProviderCode)
                .ToDictionary(x => x.Key, x => x.OrderByDescending(y => y.UpdatedAt).First());
            _logger.Information($" - {orgEnrichments.Count()} orgEnrichments");

            var courseMapper = new CourseMapper();
            var converter = new EnrichmentConverter();

            var mappedCourses = new List<SearchAndCompare.Domain.Models.Course>();

            _logger.Information("Combine courses with provider and enrichment data");

            foreach (var c in courses)
            {
                var courseEnrichment = c.CourseEnrichments
                    .OrderByDescending(y => y.UpdatedAt)
                    .FirstOrDefault(x => x.Status == EnumStatus.Published);

                var mappedCourse = courseMapper.MapToSearchAndCompareCourse(
                    c.Provider,
                    c,
                    converter.Convert(orgEnrichments.GetValueOrDefault(c.Provider.ProviderCode))?.EnrichmentModel,
                    converter.Convert(courseEnrichment)?.EnrichmentModel
                );

                if (!mappedCourse.Campuses.Any())
                {
                    // only publish running courses
                    continue;
                }

                if (!mappedCourse.CourseSubjects.Any())
                {
                    _logger.Warning(
                        $"failed to assign subject to [{c.Provider.ProviderCode}]/[{c.CourseCode}] {c.Name}. UCAS tags: {c.Subjects}");
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

        private static Logger GetLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo
                .ApplicationInsightsTraces(configuration["APPINSIGHTS_INSTRUMENTATIONKEY"])
                .CreateLogger();
        }

        private ManageCoursesDbContext GetContext()
        {
            var connectionString = _mcConfig.BuildConnectionString();

            var options = new DbContextOptionsBuilder<ManageCoursesDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            return new ManageCoursesDbContext(options);
        }

        private static McConfig GetMcConfig(IConfiguration configurationRoot)
        {
            var mcConfig = new McConfig(configurationRoot);
            mcConfig.Validate();
            return mcConfig;
        }
    }
}
