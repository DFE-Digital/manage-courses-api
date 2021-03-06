using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Api.Model;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Api.Controllers
{


    [Route("api/[controller]")]
    public class AccessRequestController : Controller
    {
        private readonly IAccessRequestService _service;

        public AccessRequestController(IAccessRequestService accessRequestService)
        {
            _service = accessRequestService;
        }

        [BearerTokenAuth]
        [HttpPost]
        [ProducesResponseType(200)]
        public StatusCodeResult Index([FromBody] AccessRequest request)
        {
            var requesterEmail = this.User.Identity.Name;
            _service.LogAccessRequest(request, requesterEmail);
            return Ok();
        }
    }
}