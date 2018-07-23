using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GovUk.Education.ManageCourses.Api.Middleware;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    
    [Authorize(AuthenticationSchemes = BearerTokenApiKeyDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class InviteController : Controller
    {
        [HttpPost]
        public IActionResult Index() 
        {
            return this.Ok();
        }
    }
}