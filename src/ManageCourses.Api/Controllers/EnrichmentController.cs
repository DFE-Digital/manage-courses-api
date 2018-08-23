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

        /// <summary>
        /// always get the latest enrichment record regardless of status
        /// </summary>
        /// <param name="ucasInstitutionCode">institution code that relates to the Ucas institution</param>
        /// <param name="ucasCourseCode">Course code that relates to the Ucas Course of the enrichment data</param>
        /// <returns>a data object. null if not found</returns>
        [HttpGet]
        [Route("institution/{ucasInstitutionCode}/course/{ucasCourseCode}")]
        public UcasCourseEnrichmentGetModel GetCourse(string ucasInstitutionCode, string ucasCourseCode)
        {
            // todo: return _service.GetCourseEnrichment(ucasCourseCode, User.Identity.Name);
            return new UcasCourseEnrichmentGetModel();
        }
        /// <summary>
        /// saves a draft enrichment record (upsert)
        /// </summary>
        /// <param name="ucasInstitutionCode"></param>
        /// <param name="ucasCourseCode">Course code that relates to the Ucas Course</param>
        /// <param name="model">contains the payload that represents the data to be saved</param>
        [HttpPost]
        [Route("institution/{ucasInstitutionCode}/course/{ucasCourseCode}")]
        public void SaveCourse(string ucasInstitutionCode, string ucasCourseCode, [FromBody] CourseEnrichmentModel model)
        {
            // todo: _service.SaveCourseEnrichment(model, ucasCourseCode, User.Identity.Name);
        }

        /// <summary>
        /// sets the status of the latest draft record to 'published'
        /// </summary>
        /// <param name="ucasInstitutionCode"></param>
        /// <param name="ucasCourseCode">Course code that relates to the Ucas Course</param>
        [HttpPost]
        [Route("institution/{ucasInstitutionCode}/course/{ucasCourseCode}/publish")]
        public void PublishCourse(string ucasInstitutionCode, string ucasCourseCode)
        {
            //TODO send to search and compare
            // todo: return _service.PublishCourseEnrichment(ucasCourseCode, User.Identity.Name);
        }
    }
}
