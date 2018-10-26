using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Services.Publish;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    [Route("api/[controller]")]
    public class PublishController : Controller
    {
        private readonly IDataService _dataService;
        private readonly ISearchAndCompareService _searchAndCompareService;
        private readonly IEnrichmentService _enrichmentservice;

        public PublishController(IDataService dataService, IEnrichmentService enrichmentservice, ISearchAndCompareService searchAndCompareService)
        {
            _dataService = dataService;
            _searchAndCompareService = searchAndCompareService;
            _enrichmentservice = enrichmentservice;
        }

        /// <summary>
        /// Publishes a single course
        /// </summary>
        /// <returns>boolean indicating success/failure</returns>
        [BearerTokenAuth]
        [HttpPost]
        [Route("course/{instCode}/{courseCode}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> PublishCourseToSearchAndCompare(string instCode, string courseCode)
        {
            var name = this.User.Identity.Name;

            var enrichmentResult = _enrichmentservice.PublishCourseEnrichment(instCode, courseCode, name);

            await _searchAndCompareService.SaveCourse(instCode, courseCode, name);

            return Ok(enrichmentResult);
        }
        /// <summary>
        /// Publishes all courses for an organisation
        /// </summary>
        /// <returns>boolean indicating success/failure</returns>
        [BearerTokenAuth]
        [HttpPost]
        [Route("organisation/{instCode}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> PublishCoursesToSearchAndCompare(string instCode)
        {
            var name = this.User.Identity.Name;

            var enrichmentResult = _enrichmentservice.PublishInstitutionEnrichment(instCode, name);

            await _searchAndCompareService.SaveCourses(instCode, name);

            return Ok(enrichmentResult);
        }
        /// <summary>
        /// Gets a generated Search and Compare course object used for Publish (to Search and Compare) and Preview
        /// This will return and unpublished (draft) record when called from Preview
        /// </summary>
        /// <returns>a single course</returns>
        [BearerTokenAuth]
        [HttpGet]
        [Route("searchandcompare/{instCode}/{courseCode}")]
        [ProducesResponseType(typeof(SearchAndCompare.Domain.Models.Course), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult GetSearchAndCompareCourse(string instCode, string courseCode)
        {
            var name = this.User.Identity.Name;
            if (string.IsNullOrWhiteSpace(instCode) || string.IsNullOrWhiteSpace(courseCode))
            {
                return BadRequest();
            }

            var courseMapper = new CourseMapper();

            var ucasInstData = _dataService.GetUcasInstitutionForUser(name, instCode);
            var orgEnrichmentData = _enrichmentservice.GetInstitutionEnrichment(instCode, name);
            var ucasCourseData = _dataService.GetCourseForUser(name, instCode, courseCode);
            var courseEnrichmentData = _enrichmentservice.GetCourseEnrichment(instCode, courseCode, name);
            if (ucasInstData == null || ucasCourseData == null)
            {
                return NotFound();
            }

            var course = courseMapper.MapToSearchAndCompareCourse(
                ucasInstData,
                ucasCourseData,
                orgEnrichmentData?.EnrichmentModel,
                courseEnrichmentData?.EnrichmentModel);

            return Ok(course);
        }
    }
}

