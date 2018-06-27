using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    [Route("api/[controller]")]
    public class DataController : Controller
    {
        private readonly IDataService _dataService;

        public DataController(IDataService dataService)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// Exports the data.
        /// </summary>
        /// <returns>The exported data</returns>
        [Authorize]
        [HttpGet]
        public OrganisationCourses Export()
        {
            var name = this.User.Identity.Name;
            var courses = _dataService.GetCoursesForUser(name);

            return courses;
        }
        /// <summary>
        /// Imports the data.
        /// </summary>
        [HttpPost]
        public void Import([FromBody] Payload payload)
        {
            _dataService.ResetDatabase();

            _dataService.ProcessPayload(payload);

            //TODO return Ok/Fail in action result
        }
    }
}

