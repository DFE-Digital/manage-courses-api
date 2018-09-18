using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Services.Publish;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    [Route("api/[controller]")]
    public class CoursesController : Controller
    {
        private readonly IDataService _dataService;
        private readonly IPublishService _publishService;
        private IEnrichmentService _enrichmentservice;

        public CoursesController(IDataService dataService, IPublishService publishService, IEnrichmentService enrichmentservice)
        {
            _dataService = dataService;
            _publishService = publishService;
            _enrichmentservice = enrichmentservice;
        }

        /// <summary>
        /// Gets a course by institution Ucas code
        /// </summary>
        /// <returns>a single course</returns>
        [BearerTokenAuth]
        [HttpGet]
        [Route("{instCode}/course/{ucasCode}")]
        [ProducesResponseType(typeof(Course), 200)]
        [ProducesResponseType(404)]
        public ActionResult Get(string instCode, string ucasCode)
        {
            var name = this.User.Identity.Name;
            var course = _dataService.GetCourse(name, instCode, ucasCode);

            if (course == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(course);
            }
        }
        /// <summary>
        /// Gets a list of course by Inst code
        /// </summary>
        /// <returns>a single course</returns>
        [BearerTokenAuth]
        [HttpGet]
        [Route("{instCode}")]
        [ProducesResponseType(typeof(InstitutionCourses), 200)]
        [ProducesResponseType(404)]
        public ActionResult GetAll(string instCode)
        {
            var name = this.User.Identity.Name;

            if (_dataService.GetUcasInstitutionForUser(name, instCode) == null)
            {
                return NotFound();
            };

            var courses = _dataService.GetCourses(name, instCode);

            return Ok(courses);
        }
        /// <summary>
        /// Publishes a single course
        /// </summary>
        /// <returns>boolean indicating success/failure</returns>
        [BearerTokenAuth]
        [HttpPost]
        [Route("publish/{instCode}/{courseCode}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult Publish(string instCode, string courseCode)
        {
            var name = this.User.Identity.Name;

            var result = _publishService.PublishCourse(instCode, courseCode, name);

            return Ok(result);
        }
        /// <summary>
        /// Gets a generated Search and Compare course
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
                return NotFound();
            }

            var courseMapper = new CourseMapper();

            var ucasInstData = _dataService.GetUcasInstitutionForUser(name, instCode);
            var orgEnrichmentData = _enrichmentservice.GetInstitutionEnrichment(instCode, name);
            var ucasCourseData = _dataService.GetCourse(name, instCode, courseCode);
            var courseEnrichmentData = _enrichmentservice.GetCourseEnrichment(instCode, courseCode, name);

            var course = courseMapper.MapToSearchAndCompareCourse(
                ucasInstData,
                ucasCourseData,
                orgEnrichmentData?.EnrichmentModel,
                courseEnrichmentData?.EnrichmentModel);

            return Ok(course);
        }
    }
}

