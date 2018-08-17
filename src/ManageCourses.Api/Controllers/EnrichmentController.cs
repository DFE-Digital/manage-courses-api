using System;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    [Authorize]
    [Route("api/enrichment")]
    public class EnrichmentController : Controller
    {
        private IEnrichmentService _service;
        public EnrichmentController(IEnrichmentService service)
        {
            _service = service;
        }
        /// <summary>
        /// always get the latest enrichment record regardless of status
        /// </summary>
        /// <param name="ucasInstitutionCode">institution code that relates to the Ucas institution of the enrichment data</param>
        /// <returns>a data object. null if not found</returns>
        [HttpGet]
        [Route("institution/{ucasInstitutionCode}")]
        public UcasInstitutionEnrichmentGetModel GetInstitution(string ucasInstitutionCode)
        {
            return _service.GetInstitutionEnrichment(ucasInstitutionCode, User.Identity.Name);
        }
        /// <summary>
        /// saves a draft enrichment record (upsert)
        /// </summary>
        /// <param name="ucasInstitutionCode">institution code that relates to the Ucas institution</param>
        /// <param name="model">contains the payload that represents the data to be saved</param>
        [HttpPost]
        [Route("institution/{ucasInstitutionCode}")]
        public void SaveInstitution(string ucasInstitutionCode, [FromBody] UcasInstitutionEnrichmentPostModel model)
        {
            _service.SaveInstitutionEnrichment(model, ucasInstitutionCode, User.Identity.Name);
        }
        /// <summary>
        /// sets the status of the latest draft record to 'published'
        /// </summary>
        /// <param name="ucasInstitutionCode">institution code that relates to the Ucas institution</param>
        /// <returns>true if successful</returns>
        [HttpPost]
        [Route("institution/{ucasInstitutionCode}/publish")]
        public bool PublishInstitution(string ucasInstitutionCode)
        {
            //TODO send to search and compare
            return _service.PublishInstitutionEnrichment(ucasInstitutionCode, User.Identity.Name);
        }
    }
}
