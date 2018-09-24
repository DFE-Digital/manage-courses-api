﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.SearchAndCompare.Domain.Client;
using GovUk.Education.SearchAndCompare.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Services.Publish
{
    public class SearchAndCompareService : ISearchAndCompareService
    {
        private readonly ISearchAndCompareApi _api;
        private readonly ICourseMapper _courseMapper;
        private readonly IDataService _dataService;
        private readonly IEnrichmentService _enrichmentService;

        public SearchAndCompareService(ISearchAndCompareApi api, ICourseMapper courseMapper, IDataService dataService, IEnrichmentService enrichmentService)
        {
            _api = api;
            _courseMapper = courseMapper;
            _dataService = dataService;
            _enrichmentService = enrichmentService;
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

            var ucasInstData = _dataService.GetUcasInstitutionForUser(email, instCode);
            var orgEnrichmentData = _enrichmentService.GetInstitutionEnrichment(instCode, email);
            var ucasCourseData = _dataService.GetCourse(email, instCode, courseCode);
            var courseEnrichmentData = _enrichmentService.GetCourseEnrichment(instCode, courseCode, email);

            var course = _courseMapper.MapToSearchAndCompareCourse(
                ucasInstData,
                ucasCourseData,
                orgEnrichmentData?.EnrichmentModel,
                courseEnrichmentData?.EnrichmentModel);

            var programmeCode = course.ProgrammeCode;
            var providerCode = course.Provider.ProviderCode;

            var result = await _api.SaveCourseAsync(course);//TODO think about logging failure

            return result;
        }
    }
}
