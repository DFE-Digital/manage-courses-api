using GovUk.Education.ManageCourses.Api.ActionFilters;
using GovUk.Education.ManageCourses.Api.Exceptions;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Api.Services.Invites;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Api.Controllers
{

    [ApiTokenAuth]
    [Route("api/[controller]")]
    public class InviteController : Controller
    {
        private readonly IInviteService _inviteService;

        public InviteController(IInviteService inviteService)
        {
            _inviteService = inviteService;
        }

        [HttpPost]
        [ExemptFromAcceptTerms]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public StatusCodeResult Index(string email)
        {
            try
            {
                _inviteService.Invite(email);
                return Ok();
            }
            catch (McUserNotFoundException)
            {
                return BadRequest();
            }
        }
    }
}
