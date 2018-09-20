using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.SearchAndCompare.Domain.Client;
using GovUk.Education.SearchAndCompare.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Services.Publish
{
    public class PublishService : IPublishService
    {
        private readonly ISearchAndCompareApi _api;
        private readonly ICourseMapper _courseMapper;
        private readonly IDataService _dataService;
        private readonly IEnrichmentService _enrichmentService;

        public PublishService(ISearchAndCompareApi api, ICourseMapper courseMapper, IDataService dataService, IEnrichmentService enrichmentService)
        {
            _api = api;
            _courseMapper = courseMapper;
            _dataService = dataService;
            _enrichmentService = enrichmentService;
        }
        /// <summary>
        /// Publishes a course to search and Compare with no authorisation email
        /// </summary>
        /// <param name="instCode"></param>
        /// <param name="courseCode"></param>
        /// <returns></returns>
        public async Task<bool> PublishCourse(string instCode, string courseCode)
        {
            if (string.IsNullOrWhiteSpace(instCode) || string.IsNullOrWhiteSpace(courseCode))
            {
                return false;
            }

            var courses = new List<Course>();
            var ucasInstData = _dataService.GetUcasInstitution(instCode);
            var orgEnrichmentData = _enrichmentService.GetInstitutionEnrichment(instCode);
            var ucasCourseData = _dataService.GetCourse(instCode, courseCode);
            var courseEnrichmentData = _enrichmentService.GetCourseEnrichment(instCode, courseCode);

            var course = _courseMapper.MapToSearchAndCompareCourse(
                ucasInstData,
                ucasCourseData,
                orgEnrichmentData?.EnrichmentModel,
                courseEnrichmentData?.EnrichmentModel);
            courses.Add(course);
            var result = await _api.SaveCoursesAsync(courses);

            return result;
        }
        /// <summary>
        /// Published a course to Search and Compare using the email address of the user
        /// </summary>
        /// <param name="instCode">institution code for the course</param>
        /// <param name="courseCode">code for the course</param>
        /// <param name="email">email of the user</param>
        /// <returns></returns>
        public async Task<bool> PublishCourse(string instCode, string courseCode, string email)
        {
            if (string.IsNullOrWhiteSpace(instCode) || string.IsNullOrWhiteSpace(courseCode) || string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var courses = new List<Course>();
            var ucasInstData = _dataService.GetUcasInstitutionForUser(email, instCode);
            var orgEnrichmentData = _enrichmentService.GetInstitutionEnrichment(instCode, email);
            var ucasCourseData = _dataService.GetCourse(email, instCode, courseCode);
            var courseEnrichmentData = _enrichmentService.GetCourseEnrichment(instCode, courseCode, email);

            var course = _courseMapper.MapToSearchAndCompareCourse(
                ucasInstData,
                ucasCourseData,
                orgEnrichmentData?.EnrichmentModel,
                courseEnrichmentData?.EnrichmentModel);
            courses.Add(course);
            var result = await _api.SaveCoursesAsync(courses);

            return result;
        }
    }
}
