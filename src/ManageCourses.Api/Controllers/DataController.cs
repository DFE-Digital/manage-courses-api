using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using Microsoft.AspNetCore.Mvc;
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

        }
    }
}
