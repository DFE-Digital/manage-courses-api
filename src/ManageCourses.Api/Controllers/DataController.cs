using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Api.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        [Route("{ucasCode}")]
        public OrganisationCourses ExportByOrganisation(string ucasCode)
        {
            var name = this.User.Identity.Name;
            var courses = _dataService.GetCoursesForUserOrganisation(name, ucasCode);

            return courses;
        }
        /// <summary>
        /// Imports the data.
        /// </summary>
        [Authorize(AuthenticationSchemes = BearerTokenApiKeyDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("ucas")]
        public void Import([FromBody] UcasPayload payload)
        {
            _dataService.ProcessUcasPayload(payload);

            //TODO return Ok/Fail in action result
        }

        /// <summary>
        /// Imports the reference data.
        /// </summary>
        [Authorize(AuthenticationSchemes = BearerTokenApiKeyDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("referencedata")]
        public void ImportReferenceData([FromBody] ReferenceDataPayload payload)
        {
            _dataService.ProcessReferencePayload(payload);

            //TODO return Ok/Fail in action result
        }
    }
}

