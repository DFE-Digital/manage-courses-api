using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    [Route("api/[controller]")]
    public class OrganisationsController : Controller
    {
        private readonly IDataService _dataService;

        public OrganisationsController(IDataService dataService)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// Gets an organisations by Provider Code
        /// </summary>
        /// <returns>a single UserOrganisation object</returns>
        [BearerTokenAuth]
        [HttpGet]
        [Route("{providerCode}")]
        [ProducesResponseType(typeof(ProviderSummary), 200)]
        [ProducesResponseType(404)]
        public ActionResult Get(string providerCode)
        {
            var name = this.User.Identity.Name;
            var organisation = _dataService.GetProviderSummaryForUser(name, providerCode);
            if (organisation == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(organisation);
            }
        }

        /// <summary>
        /// Gets a list of organisations for the user
        /// </summary>
        /// <returns>a list of UserOrganisation objects</returns>
        [BearerTokenAuth]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProviderSummary>), 200)]
        [ProducesResponseType(401)]
        public IActionResult GetAll()
        {
            var name = this.User.Identity.Name;
            var organisations = _dataService.GetProviderSummariesForUser(name);
            if (!organisations.Any())
            {
                return Unauthorized();
            }

            return Ok(organisations);
        }
    }
}

