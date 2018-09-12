using System;
using GovUk.Education.ManageCourses.Api.ActionFilters;
using GovUk.Education.ManageCourses.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    /// <summary>
    /// For supportability in operational environment.
    /// Use the actions in here to verify that various error conditions are correctly logged / alerted etc.
    /// </summary>
    [Route("api/[controller]")]
    [ApiTokenAuth]
    public class DevOpsController : Controller
    {
        /// <summary>
        /// easily searchable string
        /// </summary>
        private const string MagicString = "Zoinks Scooby!";

        private readonly ILogger<DevOpsController> _logger;

        public DevOpsController(ILogger<DevOpsController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Hit this to test exception logging. Always throws.
        /// </summary>
        [Route("throw")]
        [ExemptFromAcceptTerms]
        public ActionResult Throw()
        {
            throw new Exception($"This is a test exception. {MagicString}");
        }

        /// <summary>
        /// Spits out log at every level for testing logging configuration.
        /// </summary>
        [Route("log")]
        [ExemptFromAcceptTerms]
        public ActionResult Log()
        {
            _logger.LogCritical($"logtests: Critical. {MagicString}");
            _logger.LogError($"logtests: Error. {MagicString}");
            _logger.LogWarning($"logtests: Warning. {MagicString}");
            _logger.LogInformation($"logtests: Info. {MagicString}");
            _logger.LogDebug($"logtests: Debug. {MagicString}");
            _logger.LogTrace($"logtests: Trace. {MagicString}");
            return Ok();
        }

        /// <summary>
        /// Hit this to test logging of server errors.
        /// No exceptions used, just returns 500 status code.
        /// </summary>
        /// <returns></returns>
        [Route("error")]
        [ExemptFromAcceptTerms]
        public ActionResult ServerError()
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
