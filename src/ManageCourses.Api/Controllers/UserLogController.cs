using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GovUk.Education.ManageCourses.Api.Services;
using Microsoft.AspNetCore.Http;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    /// <summary>
    /// The user log controller.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Authorize]
    [Route("api/[controller]")]
    public class UserLogController : Controller
    {
        private readonly IUserLogService _userLogService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserLogController"/> class.
        /// </summary>
        /// <param name="userLogService">The user log service.</param>
        public UserLogController(IUserLogService userLogService)
        {
            _userLogService = userLogService;
        }

        /// <summary>
        /// Records the user that is logged in.
        /// </summary>
        /// <returns>200 or 500 http status code</returns>
        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var signInUserId = this.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var email = this.User.FindFirst(x => x.Type == ClaimTypes.Email).Value;
            var result = _userLogService.CreateOrUpdateUserLog(signInUserId, email);

            var code = result ? StatusCodes.Status200OK : StatusCodes.Status500InternalServerError;

            return StatusCode(code);
        }
    }
}