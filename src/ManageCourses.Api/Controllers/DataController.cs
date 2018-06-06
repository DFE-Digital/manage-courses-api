using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ManageCourses.Api.Controllers
{
    [Route("api/[controller]")]
    public class DataController : Controller
    {
        /// <summary>
        /// Exports the data.
        /// </summary>
        /// <returns>The exported data</returns>
        [HttpGet]
        public IEnumerable<string> Export()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// Imports the data.
        /// </summary>
        [HttpPost]
        public void Import([FromBody]IEnumerable<string> value)
        {
        }
    }
}
