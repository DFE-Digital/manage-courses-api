using System;
using GovUk.Education.ManageCourses.Api.ActionFilters;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    /// <summary>
    /// For supportability in operational environment.
    /// Use the actions in here to verify that various error conditions are correctly logged / alerted etc.
    /// </summary>
    [Route("api/[controller]")]
    public class DevOpsController : Controller
    {
        private readonly IManageCoursesDbContext _context;

        /// <summary>
        /// easily searchable string
        /// </summary>
        private const string MagicString = "Zoinks Scooby!";

        private readonly ILogger<DevOpsController> _logger;

        public DevOpsController(ILogger<DevOpsController> logger, IManageCoursesDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Hit this to test exception logging. Always throws.
        /// </summary>
        [Route("throw")]
        [ExemptFromAcceptTerms]
        [ApiTokenAuth]
        public ActionResult Throw()
        {
            throw new Exception($"This is a test exception. {MagicString}");
        }

        /// <summary>
        /// Spits out log at every level for testing logging configuration.
        /// </summary>
        [Route("log")]
        [ExemptFromAcceptTerms]
        [ApiTokenAuth]
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
        [ApiTokenAuth]
        public ActionResult ServerError()
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Simple test that this service is up and connected to the database
        /// </summary>
        /// <returns></returns>
        [Route("/ping")]
        [ExemptFromAcceptTerms]
        public ActionResult Ping()
        {
            var courseCount = _context.Courses.Count();
            const int minCourses = 1000;
            if (courseCount < minCourses)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Not enough courses found, expected at least {minCourses}. Found {courseCount} courses");
            }
            return Ok($"{courseCount} courses");
        }
    }
}
