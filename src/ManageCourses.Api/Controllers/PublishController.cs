﻿using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Services.Publish;
using Microsoft.AspNetCore.Mvc;
using GovUk.Education.ManageCourses.Api.ActionFilters;
using System;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    [Route("api/[controller]")]
    public class PublishController : Controller
    {
        private readonly IDataService _dataService;
        private readonly IManageCoursesBackendService _manageCoursesBackendService;
        private readonly ISearchAndCompareService _searchAndCompareService;
        private readonly IEnrichmentService _enrichmentservice;
        private readonly ITransitionService _transitionService;

        public PublishController(IDataService dataService, IEnrichmentService enrichmentservice, ITransitionService transitionService, ISearchAndCompareService searchAndCompareService, IManageCoursesBackendService manageCoursesBackendService)
        {
            _dataService = dataService;
            _searchAndCompareService = searchAndCompareService;
            _manageCoursesBackendService = manageCoursesBackendService;
            _enrichmentservice = enrichmentservice;
            _transitionService = transitionService;
        }

        /// <summary>
        /// Publishes a single course
        /// </summary>
        /// <returns>boolean indicating success/failure</returns>
        [BearerTokenAuth]
        [HttpPost]
        [Route("course/{providerCode}/{courseCode}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> PublishCourseToSearchAndCompare(string providerCode, string courseCode)
        {
            var name = this.User.Identity.Name;

            // update course status from new to running only if provider is opted in
            _transitionService.UpdateNewCourse(providerCode, courseCode, name);

            var enrichmentResult = _enrichmentservice.PublishCourseEnrichment(providerCode, courseCode, name);

            await _manageCoursesBackendService.SaveCourse(providerCode, courseCode, name);

            return Ok(enrichmentResult);
        }

        /// <summary>
        /// Publishes a single course
        /// </summary>
        /// <returns>boolean indicating success/failure</returns>
        [HttpPost]
        [Route("internal/course/{providerCode}/{courseCode}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ExemptFromAcceptTerms]
        [BackendApiTokenAuth]
        [Obsolete("This endpoint should not be used")]
        public async Task<ActionResult> InternalPublishCourseToSearchAndCompare(string providerCode, string courseCode, [FromBody]BackendRequest request)
        {
            var result = await _searchAndCompareService.SaveCourse(providerCode, courseCode, request.Email);

            return Ok(new{result});
        }

        /// <summary>
        /// Publishes all of a provider's courses
        /// </summary>
        /// <returns>boolean indicating success/failure</returns>
        [HttpPost]
        [Route("internal/courses/{providerCode}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ExemptFromAcceptTerms]
        [BackendApiTokenAuth]
        [Obsolete("This endpoint should not be used")]
        public async Task<ActionResult> InternalPublishCoursesToSearchAndCompare(string providerCode, [FromBody]BackendRequest request)
        {
            var result = await _searchAndCompareService.SaveCourses(providerCode, request.Email);

            return Ok(new{result});
        }

        /// <summary>
        /// Publishes all courses for an organisation
        /// </summary>
        /// <returns>boolean indicating success/failure</returns>
        [BearerTokenAuth]
        [HttpPost]
        [Route("organisation/{providerCode}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> PublishCoursesToSearchAndCompare(string providerCode)
        {
            var name = this.User.Identity.Name;

            var enrichmentResult = _enrichmentservice.PublishProviderEnrichment(providerCode, name);

            await _manageCoursesBackendService.SaveCourses(providerCode, name);

            return Ok(enrichmentResult);
        }

        /// <summary>
        /// Gets a generated Search and Compare course object used for Publish (to Search and Compare) and Preview
        /// This will return and unpublished (draft) record when called from Preview
        /// </summary>
        /// <returns>a single course</returns>
        [BearerTokenAuth]
        [HttpGet]
        [Route("searchandcompare/{providerCode}/{courseCode}")]
        [ProducesResponseType(typeof(SearchAndCompare.Domain.Models.Course), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult GetSearchAndCompareCourse(string providerCode, string courseCode)
        {
            var name = this.User.Identity.Name;
            if (string.IsNullOrWhiteSpace(providerCode) || string.IsNullOrWhiteSpace(courseCode))
            {
                return BadRequest();
            }

            var courseMapper = new CourseMapper();

            var providerData = _dataService.GetProviderForUser(name, providerCode);
            var orgEnrichmentData = _enrichmentservice.GetProviderEnrichment(providerCode, name);
            var courseData = _dataService.GetCourseForUser(name, providerCode, courseCode);
            var courseEnrichmentData = _enrichmentservice.GetCourseEnrichment(providerCode, courseCode, name);
            if (providerData == null || courseData == null)
            {
                return NotFound();
            }

            var course = courseMapper.MapToSearchAndCompareCourse(
                providerData,
                courseData,
                orgEnrichmentData?.EnrichmentModel,
                courseEnrichmentData?.EnrichmentModel);

            return Ok(course);
        }
    }
}

