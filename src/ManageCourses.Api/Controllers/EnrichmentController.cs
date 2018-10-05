using System;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    [BearerTokenAuth]
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
        [ProducesResponseType(typeof(UcasInstitutionEnrichmentGetModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult GetInstitution(string ucasInstitutionCode)
        {
            return Handle(() => _service.GetInstitutionEnrichment(ucasInstitutionCode, User.Identity.Name, false));
        }

        /// <summary>
        /// saves a draft enrichment record (upsert)
        /// </summary>
        /// <param name="ucasInstitutionCode">institution code that relates to the Ucas institution</param>
        /// <param name="model">contains the payload that represents the data to be saved</param>
        [HttpPost]
        [Route("institution/{ucasInstitutionCode}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult SaveInstitution(string ucasInstitutionCode, [FromBody] UcasInstitutionEnrichmentPostModel model)
        {
            return HandleVoid(() => _service.SaveInstitutionEnrichment(model, ucasInstitutionCode, User.Identity.Name));
        }
 
        /// <summary>
        /// always get the latest enrichment record regardless of status
        /// </summary>
        /// <param name="ucasInstitutionCode">institution code that relates to the Ucas institution</param>
        /// <param name="ucasCourseCode">Course code that relates to the Ucas Course of the enrichment data</param>
        /// <returns>a data object. null if not found</returns>
        [HttpGet]
        [Route("institution/{ucasInstitutionCode}/course/{ucasCourseCode}")]
        [ProducesResponseType(typeof(UcasCourseEnrichmentGetModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult GetCourse(string ucasInstitutionCode, string ucasCourseCode)
        {
            return Handle(() => _service.GetCourseEnrichment(ucasInstitutionCode, ucasCourseCode, User.Identity.Name, false));
        }
        /// <summary>
        /// saves a draft enrichment record (upsert)
        /// </summary>
        /// <param name="ucasInstitutionCode"></param>
        /// <param name="ucasCourseCode">Course code that relates to the Ucas Course</param>
        /// <param name="model">contains the payload that represents the data to be saved</param>
        [HttpPost]
        [Route("institution/{ucasInstitutionCode}/course/{ucasCourseCode}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult SaveCourse(string ucasInstitutionCode, string ucasCourseCode, [FromBody] CourseEnrichmentModel model)
        {
            return HandleVoid(() => _service.SaveCourseEnrichment(model, ucasInstitutionCode, ucasCourseCode, User.Identity.Name));
        }


        private ActionResult HandleVoid(Action toHandle)
        {
            return HandleImpl(() =>
            {
                toHandle();
                return Ok();
            });
        }
        private ActionResult Handle<T>(Func<T> toHandle)
        {
            return HandleImpl(() => Ok(toHandle()));
        }
        private ActionResult HandleImpl(Func<ActionResult> toHandle)
        {
            try
            {
                var res = toHandle();
                return res;
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }
    }
}
