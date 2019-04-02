using System;
using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    [Route("api/[controller]")]
    public class OrganisationsController : Controller
    {
        private readonly IManageCoursesDbContext _context;
        private readonly IEnrichmentService _enrichmentService;

        public OrganisationsController(IManageCoursesDbContext context, IEnrichmentService enrichmentService)
        {
            _context = context;
            _enrichmentService = enrichmentService;
        }

        /// <summary>
        /// Gets an organisations by Provider Code
        /// </summary>
        /// <returns>a single ProviderSummary object</returns>
        [BearerTokenAuth]
        [HttpGet]
        [Route("{providerCode}")]
        [ProducesResponseType(typeof(ProviderSummary), 200)]
        [ProducesResponseType(404)]
        public ActionResult Get(string providerCode)
        {
            var name = this.User.Identity.Name;
            var organisation = GetProviderSummaryForUser(name, providerCode);
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
        /// <returns>a list of ProviderSummary objects</returns>
        [BearerTokenAuth]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProviderSummary>), 200)]
        [ProducesResponseType(401)]
        public IActionResult GetAll()
        {
            var name = this.User.Identity.Name;
            var organisations = GetProviderSummariesForUser(name);
            if (!organisations.Any())
            {
                return Unauthorized();
            }

            return Ok(organisations);
        }

        private IEnumerable<ProviderSummary> GetProviderSummariesForUser(string email)
        {
            var providerSummaries = _context.GetOrganisationProviders(email)
                .Select(providerSummary => new ProviderSummary()
                {
                    ProviderName = providerSummary.Provider.ProviderName,
                    ProviderCode = providerSummary.Provider.ProviderCode,
                    OptedIn = providerSummary.Provider.OptedIn,
                    TotalCourses = providerSummary.Provider.Courses.Select(c => c.CourseCode).Distinct().Count()
                }).OrderBy(x => x.ProviderName).ToList();

            return providerSummaries;
        }

        private ProviderSummary Mapping(OrganisationProvider organisationProvider, string email, string providerCode) {

        if (organisationProvider != null)
            {
                var enrichment = _enrichmentService.GetProviderEnrichment(providerCode, email);

                return new ProviderSummary()
                {
                    ProviderName = organisationProvider.Provider.ProviderName,
                    ProviderCode = organisationProvider.Provider.ProviderCode,
                    OptedIn = organisationProvider.Provider.OptedIn,
                    TotalCourses = organisationProvider.Provider.Courses.Select(c => c.CourseCode).Distinct()
                        .Count(),
                    EnrichmentWorkflowStatus = enrichment?.Status
                };
            }

            return null;
        }

        private ProviderSummary GetProviderSummaryForUser(string email, string providerCode)
        {
            var organisationProvider = _context.GetOrganisationProvider(email, providerCode, Mapping);

            return organisationProvider;
        }
    }
}

