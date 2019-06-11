using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.SearchAndCompare.Domain.Client;
using Microsoft.Extensions.Logging;
using Course = GovUk.Education.SearchAndCompare.Domain.Models.Course;

namespace GovUk.Education.ManageCourses.Api.Services.Publish
{
    public class SearchAndCompareService : ISearchAndCompareService
    {
        private readonly ISearchAndCompareApi _api;
        private readonly ICourseMapper _courseMapper;
        private readonly IDataService _dataService;
        private readonly IEnrichmentService _enrichmentService;
        private readonly ILogger _logger;

        public SearchAndCompareService(ISearchAndCompareApi api, ICourseMapper courseMapper, IDataService dataService, IEnrichmentService enrichmentService, ILogger<SearchAndCompareService> logger)
        {
            _api = api;
            _courseMapper = courseMapper;
            _dataService = dataService;
            _enrichmentService = enrichmentService;
            _logger = logger;
        }
        /// <summary>
        /// Publishes a list of courses to Search and Compare
        /// </summary>
        /// <param name="providerCode">provider code for the courses</param>
        /// <param name="email">email of the user</param>
        /// <returns></returns>
        public async Task<bool> SaveCourses(string providerCode, string email)
        {
            if (string.IsNullOrWhiteSpace(providerCode) || string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var courses = GetValidCourses(providerCode, email);

            return await SaveImplementation(courses, providerCode);
        }
        /// <summary>
        /// Publishes a list of courses to Search and Compare
        /// </summary>
        /// <param name="providerCode">provider code for the courses</param>
        /// <param name="courseCode">code for the course (if a single course is to be published)</param>
        /// <param name="email">email of the user</param>
        /// <returns></returns>
        public async Task<bool> SaveCourse(string providerCode, string courseCode, string email)
        {
            if (string.IsNullOrWhiteSpace(providerCode) || string.IsNullOrWhiteSpace(courseCode) || string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var courses = GetValidCourses(providerCode, email, courseCode);

            return await SaveImplementation(courses, providerCode);
        }

        private List<Course> GetValidCourses(string providerCode, string email, string courseCode = null)
        {
            var ucasProviderData = _dataService.GetProviderForUser(email, providerCode);
            var orgEnrichmentData = _enrichmentService.GetProviderEnrichmentForPublish(providerCode, email);

            var courses = new List<Course> ();

            if (courseCode != null)
            {
                courses = new List<Course> { GetCourse(providerCode, courseCode, email, ucasProviderData, orgEnrichmentData) };

            }
            else
            {
                courses.AddRange(_dataService.GetCoursesForUser(email, providerCode)
                    .Select(x => GetCourse(providerCode, x.CourseCode, email, ucasProviderData, orgEnrichmentData)));
            }

            return courses.Where(courseToSave => courseToSave.IsValid(false) && courseToSave.Campuses.Any()).ToList();
        }

        private async Task<bool> SaveImplementation(IList<Course> courses, string providerCode)
        {
            try
            {
                if (courses.Count == 0)
                {
                    _logger.LogInformation($"Save courses to search and compare; no courses for provider: {providerCode}");
                    return false;
                }

                if (!await _api.UpdateCoursesAsync(courses))
                {
                    _logger.LogError($"Save courses to search and compare failed for provider: {providerCode}");
                    return false;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An unexpected error occured. Save courses to search and compare failed for provider: {providerCode}");
                return false;
            }
            return true;
        }

        private Course GetCourse(string providerCode, string courseCode, string email, Provider ucasProviderData, UcasProviderEnrichmentGetModel orgEnrichmentData)
        {
            var ucasCourseData = _dataService.GetCourseForUser(email, providerCode, courseCode);
            var courseEnrichmentData = _enrichmentService.GetCourseEnrichmentForPublish(providerCode, courseCode, email);

            var courseToReturn = _courseMapper.MapToSearchAndCompareCourse(
                ucasProviderData,
                ucasCourseData,
                orgEnrichmentData.EnrichmentModel,
                courseEnrichmentData?.EnrichmentModel);

            return courseToReturn;
        }
    }
}
