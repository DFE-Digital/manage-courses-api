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
        /// <param name="ucasInstitutionCode">institution code that relates to the Ucas institution</param>
        /// <returns></returns>
        [HttpGet]
        [Route("institution/{ucasInstitutionCode}")]
        public UcasInstitutionEnrichmentGetModel GetLatestInstitution(string ucasInstitutionCode)
        {
            return _service.GetInstitutionEnrichment(ucasInstitutionCode, User.Identity.Name);
        }
        /// <summary>
        /// saves an enrichment record (always draft)
        /// </summary>
        /// <param name="ucasInstitutionCode">institution code that relates to the Ucas institution</param>
        /// <param name="model">containds the payload that represents the records to be saved</param>
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
        /// <returns>Ok if successful. Bad request if unsuccessful</returns>
        [HttpPost]
        [Route("institution/{ucasInstitutionCode}/publish")]
        public ActionResult Publish(string ucasInstitutionCode)
        {            
            try
            {
                //TODO send to search and compare
                var result = _service.PublishInstitutionEnrichment(ucasInstitutionCode, User.Identity.Name);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
