using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using Microsoft.AspNetCore.Mvc;
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
        public void Import ([FromBody] IEnumerable<string> value) { }
    }
}
