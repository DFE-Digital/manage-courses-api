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
        /// <param name="providerCode">provider code that relates to the provider of the enrichment data</param>
        /// <returns>a data object. null if not found</returns>
        [HttpGet]
        [Route("provider/{providerCode}")]
        [ProducesResponseType(typeof(UcasProviderEnrichmentGetModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult GetProvider(string providerCode)
        {
            return Handle(() => _service.GetProviderEnrichment(providerCode, User.Identity.Name));
        }

        /// <summary>
        /// saves a draft enrichment record (upsert)
        /// </summary>
        /// <param name="providerCode">provider code that relates to the Ucas provider</param>
        /// <param name="model">contains the payload that represents the data to be saved</param>
        [HttpPost]
        [Route("provider/{providerCode}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult SaveProvider(string providerCode, [FromBody] UcasProviderEnrichmentPostModel model)
        {
            return HandleVoid(() => _service.SaveProviderEnrichment(model, providerCode, User.Identity.Name));
        }
 
        /// <summary>
        /// always get the latest enrichment record regardless of status
        /// </summary>
        /// <param name="providerCode">provider code that relates to the Ucas provider</param>
        /// <param name="courseCode">Course code that relates to the Ucas Course of the enrichment data</param>
        /// <returns>a data object. null if not found</returns>
        [HttpGet]
        [Route("provider/{providerCode}/course/{courseCode}")]
        [ProducesResponseType(typeof(UcasCourseEnrichmentGetModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult GetCourse(string providerCode, string courseCode)
        {
            return Handle(() => _service.GetCourseEnrichment(providerCode, courseCode, User.Identity.Name));
        }
        /// <summary>
        /// saves a draft enrichment record (upsert)
        /// </summary>
        /// <param name="providerCode"></param>
        /// <param name="courseCode">Course code that relates to the Ucas Course</param>
        /// <param name="model">contains the payload that represents the data to be saved</param>
        [HttpPost]
        [Route("provider/{providerCode}/course/{courseCode}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult SaveCourse(string providerCode, string courseCode, [FromBody] CourseEnrichmentModel model)
        {
            return HandleVoid(() => _service.SaveCourseEnrichment(model, providerCode, courseCode, User.Identity.Name));
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
