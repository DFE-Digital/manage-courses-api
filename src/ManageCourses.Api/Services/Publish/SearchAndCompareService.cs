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
        /// <param name="instCode">institution code for the courses</param>
        /// <param name="email">email of the user</param>
        /// <returns></returns>
        public async Task<bool> SaveCourses(string instCode, string email)
        {
            if (string.IsNullOrWhiteSpace(instCode) || string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var courses = GetValidCourses(instCode, email);

            return await SaveImplementation(courses, instCode);
        }
        /// <summary>
        /// Publishes a list of courses to Search and Compare
        /// </summary>
        /// <param name="instCode">institution code for the courses</param>
        /// <param name="courseCode">code for the course (if a single course is to be published)</param>
        /// <param name="email">email of the user</param>
        /// <returns></returns>
        public async Task<bool> SaveCourse(string instCode, string courseCode, string email)
        {
            if (string.IsNullOrWhiteSpace(instCode) || string.IsNullOrWhiteSpace(courseCode) || string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var courses = GetValidCourses(instCode, email, courseCode);

            return await SaveImplementation(courses, instCode);
        }

        private List<Course> GetValidCourses(string instCode, string email, string courseCode = null)
        {
            var ucasInstData = _dataService.GetUcasInstitutionForUser(email, instCode);
            var orgEnrichmentData = _enrichmentService.GetInstitutionEnrichmentForPublish(instCode, email);

            var courses = new List<Course> ();

            if (courseCode != null)
            {
                courses = new List<Course> { GetCourse(instCode, courseCode, email, ucasInstData, orgEnrichmentData) };

            }
            else
            {
                courses.AddRange(_dataService.GetCoursesForUser(email, instCode)
                    .Select(x => GetCourse(instCode, x.CourseCode, email, ucasInstData, orgEnrichmentData)));
            }

            return courses.Where(courseToSave => courseToSave.IsValid(false)).ToList();
        }

        private async Task<bool> SaveImplementation(List<Course> courses, string instCode)
        {
            var returnBool = false;
            try
            {
                if (courses.Count > 0)
                {
                    returnBool = await _api.UpdateCoursesAsync(courses);
                }

                if (!returnBool)
                {
                    _logger.LogError($"Save courses to search and compare failed for institution: {instCode}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An unexpected error occured. Save courses to search and compare failed for institution: {instCode}");
            }

            return returnBool;
        }

        private Course GetCourse(string instCode, string courseCode, string email, Provider ucasInstData, UcasInstitutionEnrichmentGetModel orgEnrichmentData)
        {
            var ucasCourseData = _dataService.GetCourseForUser(email, instCode, courseCode);
            var courseEnrichmentData = _enrichmentService.GetCourseEnrichmentForPublish(instCode, courseCode, email);

            var courseToReturn = _courseMapper.MapToSearchAndCompareCourse(
                ucasInstData,
                ucasCourseData,
                orgEnrichmentData.EnrichmentModel,
                courseEnrichmentData?.EnrichmentModel);

            return courseToReturn;
        }
    }
}
