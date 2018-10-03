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
        private readonly IPgdeWhitelist _pgdeWhitelist;

        public SearchAndCompareService(ISearchAndCompareApi api, ICourseMapper courseMapper, IDataService dataService, IEnrichmentService enrichmentService, ILogger<SearchAndCompareService> logger, IPgdeWhitelist pgdeWhitelist)
        {
            _api = api;
            _courseMapper = courseMapper;
            _dataService = dataService;
            _enrichmentService = enrichmentService;
            _logger = logger;
            _pgdeWhitelist = pgdeWhitelist;
        }

        ///// <summary>
        ///// Published a course to Search and Compare using the email address of the user
        ///// </summary>
        ///// <param name="instCode">institution code for the course</param>
        ///// <param name="courseCode">code for the course</param>
        ///// <param name="email">email of the user</param>
        ///// <returns></returns>
        //public async Task<bool> SaveSingleCourseToSearchAndCompare(string instCode, string courseCode, string email)
        // {
        //    if (string.IsNullOrWhiteSpace(instCode) || string.IsNullOrWhiteSpace(courseCode) || string.IsNullOrWhiteSpace(email))
        //     {
        //         return false;
        //     }
        //     var returnBool = false;
        //     try
        //     {
        //         var ucasInstData = _dataService.GetUcasInstitutionForUser(email, instCode);
        //         var orgEnrichmentData = _enrichmentService.GetInstitutionEnrichment(instCode, email, true);

        //         var course = GetCourse(instCode, courseCode, email, ucasInstData, orgEnrichmentData);

        //         if (course.IsValid(true))
        //         {
        //             var courseList = new List<Course> { course };
        //             returnBool = await _api.SaveCoursesAsync(courseList);
        //         }
        //         if (!returnBool)
        //         {
        //             _logger.LogError($"Save course to search and compare failed for course: {courseCode}, Institution: {instCode}");
        //         }
        //         else
        //         {
        //             _logger.LogError($"Save course to search and compare failed for course because the course status was draft: {courseCode}, Institution: {instCode}");
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         _logger.LogError(e, $"An unexpected error occured. Save course to search and compare failed for course: {courseCode}, Institution: {instCode}");
        //     }

        //     return returnBool;
        //}
        /// <summary>
        /// Publishes a list of courses to Search and Compare
        /// </summary>
        /// <param name="instCode">institution code for the courses</param>
        /// <param name="email">email of the user</param>
        /// <returns></returns>
        public async Task<bool> SaveCourses(string instCode, string email)
        {
            var courseCodes = _dataService.GetCourses(email, instCode).Courses
                .Select(x => new CourseCode {InstCode = x.InstCode, CrseCode = x.CourseCode})
                .ToList();
            return await SaveImplementation(courseCodes, email);
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
            return await SaveImplementation(
                new List<CourseCode>
                {
                    new CourseCode {InstCode = instCode.ToUpperInvariant(), CrseCode = courseCode.ToUpperInvariant()}
                }, email);

        }
        private async Task<bool> SaveImplementation(IReadOnlyCollection<CourseCode> courseCodes, string email)
        {
            var returnBool = false;
            var instCode = courseCodes.Select(x => x.InstCode).FirstOrDefault();

            try
            {
                var ucasInstData = _dataService.GetUcasInstitutionForUser(email, instCode);
                var orgEnrichmentData = _enrichmentService.GetInstitutionEnrichment(instCode, email, true);

                var coursesToSave = courseCodes.Select(course => GetCourse(instCode, course.CrseCode, email, ucasInstData, orgEnrichmentData))
                    .Where(courseToSave => courseToSave.IsValid(false))
                    .ToList();

                if (coursesToSave.Count > 0)
                {
                    returnBool = await _api.SaveCoursesAsync(coursesToSave);
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
        private Course GetCourse(string instCode, string courseCode, string email, UcasInstitution ucasInstData, UcasInstitutionEnrichmentGetModel orgEnrichmentData)
        {
            var ucasCourseData = _dataService.GetCourse(email, instCode, courseCode);
            var courseEnrichmentData = _enrichmentService.GetCourseEnrichment(instCode, courseCode, email, true);

            var courseToReturn = _courseMapper.MapToSearchAndCompareCourse(
                ucasInstData,
                ucasCourseData,
                orgEnrichmentData.EnrichmentModel,
                courseEnrichmentData?.EnrichmentModel);

            return courseToReturn;
        }
    }
}
