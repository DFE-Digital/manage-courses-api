using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
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
        /// Gets an organisations by Institution Code
        /// </summary>
        /// <returns>a single UserOrganisation object</returns>
        [Authorize]
        [HttpGet]
        [Route("{instCode}")]
        [ProducesResponseType(typeof(UserOrganisation), 200)]
        [ProducesResponseType(404)]
        public ActionResult Get(string instCode)
        {
            var name = this.User.Identity.Name;
            var organisation = _dataService.GetOrganisationForUser(name, instCode);
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
        /// Gets UCAS Institution data by Institution Code
        /// </summary>
        /// <returns>a single UcasInstitution object</returns>
        [Authorize]
        [HttpGet]
        [Route("{instCode}/ucas")]
        [ProducesResponseType(typeof(UcasInstitution), 200)]
        [ProducesResponseType(404)]
        public ActionResult GetUcasInstitution(string instCode)
        {
            var name = this.User.Identity.Name;
            var organisation = _dataService.GetUcasInstitutionForUser(name, instCode);
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
        [Authorize]
        [HttpGet]        
        [ProducesResponseType(typeof(IEnumerable<UserOrganisation>), 200)]
        [ProducesResponseType(401)]
        public IActionResult GetAll()
        {
            var name = this.User.Identity.Name;
            var organisations = _dataService.GetOrganisationsForUser(name);
            if (!organisations.Any())
            {
                return Unauthorized();
            }

            return Ok(organisations);
        }
    }
}

