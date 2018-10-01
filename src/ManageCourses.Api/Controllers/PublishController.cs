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
        private readonly IPgdeWhitelist _pgdeWhitelist;

        public PublishController(IDataService dataService, IEnrichmentService enrichmentservice, ISearchAndCompareService searchAndCompareService, IPgdeWhitelist pgdeWhitelist)
        {
            _dataService = dataService;
            _searchAndCompareService = searchAndCompareService;
            _pgdeWhitelist = pgdeWhitelist;
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
        public async Task<ActionResult> PublishToSearchAndCompare(string instCode, string courseCode)
        {
            var name = this.User.Identity.Name;

            var enrichmentResult = _enrichmentservice.PublishCourseEnrichment(instCode, courseCode, name);

            //await _searchAndCompareService.SaveSingleCourseToSearchAndCompare(instCode, courseCode, name);

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
            var orgEnrichmentData = _enrichmentservice.GetInstitutionEnrichment(instCode, name, false);
            var ucasCourseData = _dataService.GetCourse(name, instCode, courseCode);
            var courseEnrichmentData = _enrichmentservice.GetCourseEnrichment(instCode, courseCode, name);
            if (ucasInstData == null || ucasCourseData == null)
            {
                return NotFound();
            }

            var isPgde = _pgdeWhitelist.IsPgde(instCode, courseCode);
            var course = courseMapper.MapToSearchAndCompareCourse(
                ucasInstData,
                ucasCourseData,
                orgEnrichmentData?.EnrichmentModel,
                courseEnrichmentData?.EnrichmentModel,
                isPgde);

            return Ok(course);
        }
    }
}

