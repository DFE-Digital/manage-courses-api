using System;
using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
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
            return _context.GetAll();
        }

        /// <summary>
        /// Imports the data.
        /// </summary>
        [HttpPost]
        public void Import([FromBody] Payload payload)
        {
            ProcessPayload(payload);
            //TODO return Ok/Fail in action result
        }

        private void ProcessPayload(Payload payload)
        {
            foreach (var course in payload.Courses)
            {
                // copy props to prevent changing id
                // todo: consider removing id from exposed API
                _context.AddCourse(new Course
                {
                    Title = course.Title,
                    UcasCode = course.UcasCode,
                    Type = course.Type,
                });
            }

            _context.Save();
        }
    }
}
