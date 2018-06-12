using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Course = GovUk.Education.ManageCourses.Domain.Models.Course;

namespace GovUk.Education.ManageCourses.Api.Controllers {

    [Route ("api/[controller]")]
    public class DataController : Controller {
        private readonly IManageCoursesDbContext _context;

        public DataController (IManageCoursesDbContext context) {
            _context = context;
        }

        /// <summary>
        /// Exports the data.
        /// </summary>
        /// <returns>The exported data</returns>
        [HttpGet]
        public IActionResult Export () {
            return Ok(new {data= _context.GetAll() });
        }

        /// <summary>
        /// Imports the data.
        /// </summary>
        [HttpPost]
        public void Import([FromBody] Payload payload)
        {
            var result = ProcessPayload(payload);
            //TODO return Ok/Fail in action result
        }

        private bool ProcessPayload(Payload payload)
        {
            try
            {
                foreach (var course in payload.Courses)
                {
                    _context.AddCourse(new Course {Title = course.Title});
                }

                _context.Save();
                return true;
            }
            catch (Exception ex)
            {
                //TODO create logger and log message
                return false;
            }
        }
    }
}
