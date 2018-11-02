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
        /// <param name="ucasProviderCode">provider code that relates to the Ucas provider of the enrichment data</param>
        /// <returns>a data object. null if not found</returns>
        [HttpGet]
        [Route("provider/{ucasProviderCode}")]
        [ProducesResponseType(typeof(UcasProviderEnrichmentGetModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult GetProvider(string ucasProviderCode)
        {
            return Handle(() => _service.GetProviderEnrichment(ucasProviderCode, User.Identity.Name));
        }

        /// <summary>
        /// saves a draft enrichment record (upsert)
        /// </summary>
        /// <param name="ucasProviderCode">provider code that relates to the Ucas provider</param>
        /// <param name="model">contains the payload that represents the data to be saved</param>
        [HttpPost]
        [Route("provider/{ucasProviderCode}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult SaveProvider(string ucasProviderCode, [FromBody] UcasProviderEnrichmentPostModel model)
        {
            return HandleVoid(() => _service.SaveProviderEnrichment(model, ucasProviderCode, User.Identity.Name));
        }
 
        /// <summary>
        /// always get the latest enrichment record regardless of status
        /// </summary>
        /// <param name="ucasProviderCode">provider code that relates to the Ucas provider</param>
        /// <param name="ucasCourseCode">Course code that relates to the Ucas Course of the enrichment data</param>
        /// <returns>a data object. null if not found</returns>
        [HttpGet]
        [Route("provider/{ucasProviderCode}/course/{ucasCourseCode}")]
        [ProducesResponseType(typeof(UcasCourseEnrichmentGetModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult GetCourse(string ucasProviderCode, string ucasCourseCode)
        {
            return Handle(() => _service.GetCourseEnrichment(ucasProviderCode, ucasCourseCode, User.Identity.Name));
        }
        /// <summary>
        /// saves a draft enrichment record (upsert)
        /// </summary>
        /// <param name="ucasProviderCode"></param>
        /// <param name="ucasCourseCode">Course code that relates to the Ucas Course</param>
        /// <param name="model">contains the payload that represents the data to be saved</param>
        [HttpPost]
        [Route("provider/{ucasProviderCode}/course/{ucasCourseCode}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult SaveCourse(string ucasProviderCode, string ucasCourseCode, [FromBody] CourseEnrichmentModel model)
        {
            return HandleVoid(() => _service.SaveCourseEnrichment(model, ucasProviderCode, ucasCourseCode, User.Identity.Name));
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
