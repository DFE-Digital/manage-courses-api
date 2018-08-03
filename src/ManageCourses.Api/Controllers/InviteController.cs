using GovUk.Education.ManageCourses.Api.Exceptions;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Api.Controllers
{

    [Authorize(AuthenticationSchemes = BearerTokenApiKeyDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class InviteController : Controller
    {
        private readonly IInviteService _inviteService;

        public InviteController(IInviteService inviteService)
        {
            _inviteService = inviteService;
        }

        [HttpPost]
        public IActionResult Index(string email)
        {
            try
            {
                _inviteService.Invite(email);
                return Ok();
            }
            catch (McUserNotFoundException)
            {
                // Using 422 because: https://stackoverflow.com/questions/3050518/what-http-status-response-code-should-i-use-if-the-request-is-missing-a-required/10323055#10323055
                Response.StatusCode = 422; // unprocessable entity (extension to http)
                return Json(new { error = "McUser not found" });
            }
        }
    }
}
