using System.Collections.Generic;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    [Route("api/[controller]")]
    public class CoursesController : Controller
    {
        private readonly IDataService _dataService;
         public CoursesController(IDataService dataService)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// Gets a course by provider code
        /// </summary>
        /// <returns>a single course</returns>
        [BearerTokenAuth]
        [HttpGet]
        [Route("{providerCode}/course/{courseCode}")]
        [ProducesResponseType(typeof(Course), 200)]
        [ProducesResponseType(404)]
        public ActionResult Get(string providerCode, string courseCode)
        {
            var name = this.User.Identity.Name;
            var course = _dataService.GetCourseForUser(name, providerCode, courseCode);

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
        /// Gets a list of course by provider code
        /// </summary>
        /// <returns>a single course</returns>
        [BearerTokenAuth]
        [HttpGet]
        [Route("{providerCode}")]
        [ProducesResponseType(typeof(List<Course>), 200)]
        [ProducesResponseType(404)]
        public ActionResult GetAll(string providerCode)
        {
            var name = this.User.Identity.Name;

            if (_dataService.GetProviderForUser(name, providerCode) == null)
            {
                return NotFound();
            };

            var courses = _dataService.GetCoursesForUser(name, providerCode);

            return Ok(courses);
        }
    }
}

