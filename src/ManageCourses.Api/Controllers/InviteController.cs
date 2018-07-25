using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Api.Services.Email;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    
    [Authorize(AuthenticationSchemes = BearerTokenApiKeyDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class InviteController : Controller
    {
        private readonly IInviteEmailService _inviteEmailService;

        public InviteController(IInviteEmailService inviteEmailService)
        {
            _inviteEmailService = inviteEmailService;
        }

        [HttpPost]
        public IActionResult Index() 
        {
            return this.Ok();
        }
    }
}