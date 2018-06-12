using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Course = GovUk.Education.ManageCourses.Domain.Models.Course;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    [Route("api/[controller]")]
    public class DataController : Controller
    {
        private readonly IManageCoursesDbContext _context;

        public DataController(IManageCoursesDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Exports the data.
        /// </summary>
        /// <returns>The exported data</returns>
        [HttpGet]
        public IEnumerable<Course> Export()
        {
            return _context.GetAll().Select(c => new Course { Title = c.Title, UcasCourseCode = c.UcasCode });
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
