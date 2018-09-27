using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.SearchAndCompare.Domain.Client;
using GovUk.Education.SearchAndCompare.Domain.Models;
using Microsoft.Extensions.Logging;

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
        /// Published a course to Search and Compare using the email address of the user
        /// </summary>
        /// <param name="instCode">institution code for the course</param>
        /// <param name="courseCode">code for the course</param>
        /// <param name="email">email of the user</param>
        /// <returns></returns>
        public async Task<bool> SaveSingleCourseToSearchAndCompare(string instCode, string courseCode, string email)
         {
            if (string.IsNullOrWhiteSpace(instCode) || string.IsNullOrWhiteSpace(courseCode) || string.IsNullOrWhiteSpace(email))
             {
                 return false;
             }
             var returnBool = false;
             try
             {
                 var ucasInstData = _dataService.GetUcasInstitutionForUser(email, instCode);
                 var orgEnrichmentData = _enrichmentService.GetInstitutionEnrichment(instCode, email, true);
                 var ucasCourseData = _dataService.GetCourse(email, instCode, courseCode);
                 var courseEnrichmentData = _enrichmentService.GetCourseEnrichment(instCode, courseCode, email);

                 if (courseEnrichmentData.Status.Equals(EnumStatus.Published))
                 {
                     var course = _courseMapper.MapToSearchAndCompareCourse(
                         ucasInstData,
                         ucasCourseData,
                         orgEnrichmentData.EnrichmentModel,
                         courseEnrichmentData?.EnrichmentModel);

                     if (course.IsValid(true))
                     {
                         returnBool = await _api.SaveCourseAsync(course);
                     }

                     if (!returnBool)
                     {
                         _logger.LogError($"Save course to search and compare failed for course: {courseCode}, Institution: {instCode}");
                     }
                }
                else
                 {
                    _logger.LogError($"Save course to search and compare failed for course because the course status was draft: {courseCode}, Institution: {instCode}");
                }
            }
            catch (Exception e)
             {
                 _logger.LogError(e, $"An unexpected error occured. Save course to search and compare failed for course: {courseCode}, Institution: {instCode}");
             }

             return returnBool;
        }
    }
}
