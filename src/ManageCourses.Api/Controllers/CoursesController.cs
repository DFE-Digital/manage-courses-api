using System.Collections.Generic;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Model;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        [HttpGet]
        [Route("{instCode}/course/{ucasCode}")]
        public Course Get(string instCode, string ucasCode)
        {
            var name = this.User.Identity.Name;
            var course = _dataService.GetCourse(name, instCode, ucasCode);

            return course;
        }
        /// <summary>
        /// Gets a list of course by Inst code
        /// </summary>
        /// <returns>a single course</returns>
        [Authorize]
        [HttpGet]
        [Route("{instCode}")]
        public InstitutionCourses GetAll(string instCode)
        {
            var name = this.User.Identity.Name;
            var course = _dataService.GetCourses(name, instCode);

            return course;
        }
    }
}

