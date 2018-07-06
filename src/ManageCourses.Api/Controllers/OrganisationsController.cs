using System.Collections.Generic;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Model;
using Microsoft.AspNetCore.Authorization;
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
        /// Exports the data.
        /// </summary>
        /// <returns>The exported data</returns>
        [Authorize]
        [HttpGet]
        public IEnumerable<UserOrganisation> Get()
        {
            var name = this.User.Identity.Name;            
            var organisations = _dataService.GetOrganisationsForUser(name);

            return organisations;
        }
    }
}

