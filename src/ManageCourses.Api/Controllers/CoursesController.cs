using System.Collections.Generic;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Services.Publish;
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
        /// Gets a course by institution Ucas code
        /// </summary>
        /// <returns>a single course</returns>
        [BearerTokenAuth]
        [HttpGet]
        [Route("{instCode}/course/{courseCode}")]
        [ProducesResponseType(typeof(Course), 200)]
        [ProducesResponseType(404)]
        public ActionResult Get(string instCode, string courseCode)
        {
            var name = this.User.Identity.Name;
            var course = _dataService.GetCourseForUser(name, instCode, courseCode);

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
        [ProducesResponseType(typeof(List<Course>), 200)]
        [ProducesResponseType(404)]
        public ActionResult GetAll(string instCode)
        {
            var name = this.User.Identity.Name;

            if (_dataService.GetUcasInstitutionForUser(name, instCode) == null)
            {
                return NotFound();
            };

            var courses = _dataService.GetCoursesForUser(name, instCode);

            return Ok(courses);
        }
    }
}

