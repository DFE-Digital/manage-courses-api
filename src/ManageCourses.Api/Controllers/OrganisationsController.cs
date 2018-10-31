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
        /// Gets an organisations by Institution Code
        /// </summary>
        /// <returns>a single UserOrganisation object</returns>
        [BearerTokenAuth]
        [HttpGet]
        [Route("{instCode}")]
        [ProducesResponseType(typeof(InstitutionSummary), 200)]
        [ProducesResponseType(404)]
        public ActionResult Get(string instCode)
        {
            var name = this.User.Identity.Name;
            var organisation = GetInstitutionSummaryForUser(name, instCode);
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
        [ProducesResponseType(typeof(IEnumerable<InstitutionSummary>), 200)]
        [ProducesResponseType(401)]
        public IActionResult GetAll()
        {
            var name = this.User.Identity.Name;
            var organisations = GetInstitutionSummariesForUser(name);
            if (!organisations.Any())
            {
                return Unauthorized();
            }

            return Ok(organisations);
        }

        private IEnumerable<InstitutionSummary> GetInstitutionSummariesForUser(string email)
        {
            var institutionSummaries = _context.GetOrganisationInstitutions(email)
                .Select(institutionSummary => new InstitutionSummary()
                {
                    InstName = institutionSummary.Institution.InstName,
                    InstCode = institutionSummary.Institution.InstCode,
                    TotalCourses = institutionSummary.Institution.Courses.Select(c => c.CourseCode).Distinct().Count()
                }).OrderBy(x => x.InstName).ToList();

            return institutionSummaries;
        }

        private InstitutionSummary Mapping(OrganisationInstitution organisationInstitution, string email, string instCode) {

        if (organisationInstitution != null)
            {
                var enrichment = _enrichmentService.GetInstitutionEnrichment(instCode, email);

                return new InstitutionSummary()
                {
                    InstName = organisationInstitution.Institution.InstName,
                    InstCode = organisationInstitution.Institution.InstCode,
                    TotalCourses = organisationInstitution.Institution.Courses.Select(c => c.CourseCode).Distinct()
                        .Count(),
                    EnrichmentWorkflowStatus = enrichment?.Status
                };
            }

            return null;
        }

        private InstitutionSummary GetInstitutionSummaryForUser(string email, string instCode)
        {
            var organisationInstitution = _context.GetOrganisationInstitution(email, instCode, Mapping);

            return organisationInstitution;
        }
    }
}

