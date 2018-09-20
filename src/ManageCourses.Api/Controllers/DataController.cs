using GovUk.Education.ManageCourses.Api.ActionFilters;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Api.Services.Publish;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    [Route("api/[controller]")]
    public class DataController : Controller
    {
        private readonly IDataService _dataService;
        private readonly IPublishService _publishService;

        public DataController(IDataService dataService, IPublishService publishService)
        {
            _dataService = dataService;
            _publishService = publishService;
        }

        /// <summary>
        /// Exports the data.
        /// </summary>
        /// <returns>The exported data</returns>
        [BearerTokenAuth]
        [HttpGet]
        [Route("{ucasCode}")]
        public OrganisationCourses ExportByOrganisation(string ucasCode)
        {
            var name = this.User.Identity.Name;
            var courses = _dataService.GetCoursesForUserOrganisation(name, ucasCode);

            return courses;
        }
        /// <summary>
        /// Imports the data.
        /// </summary>
        [ApiTokenAuth]
        [ExemptFromAcceptTerms]
        [HttpPost]
        [Route("ucas")]
        public void Import([FromBody] UcasPayload payload)
        {
            _dataService.ProcessUcasPayload(payload);
            PublishCourses();
        }
        /// <summary>
        /// Imports the reference data.
        /// </summary>
        [ApiTokenAuth]
        [ExemptFromAcceptTerms]
        [HttpPost]
        [Route("referencedata")]
        public void ImportReferenceData([FromBody] ReferenceDataPayload payload)
        {
            _dataService.ProcessReferencePayload(payload);

            //TODO return Ok/Fail in action result
        }

        private void PublishCourses()
        {
            var courses = _dataService.GetAllCourses();

            _publishService.PublishCourses(courses);
            // foreach (var course in _dataService.GetAllCourses())
            // {
            //     _publishService.PublishCourse(course.InstCode, course.CourseCode);
            //     break;//just one for the moment
            // }
        }
    }
}

