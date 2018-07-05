using System;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GovUk.Education.ManageCourses.Api.Services;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UserLogController : Controller
    {
        private readonly IUserLogService _userLogService;

        public UserLogController(IUserLogService userLogService)
        {
            _userLogService = userLogService;
        }

        [HttpPost]
        public async Task<StatusCodeResult> Index()
        {
            var signInUserId = this.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var email = this.User.FindFirst(x => x.Type == ClaimTypes.Email).Value;
            var result = _userLogService.CreateOrUpdateUserLog(signInUserId, email);

            var code = result ? 200 : 500;

            return StatusCode(code);
        }
    }
}