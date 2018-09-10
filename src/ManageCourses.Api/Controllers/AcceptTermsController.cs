using System;
using System.Linq;
using GovUk.Education.ManageCourses.Api.ActionFilters;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Api.Controllers
{   
    [Route("api/[controller]")]
    public class AcceptTermsController : Controller
    {
        private readonly IManageCoursesDbContext context;

        public AcceptTermsController(IManageCoursesDbContext context)
        {
            this.context = context;
        }

        [Authorize]
        [ExemptFromAcceptTerms]
        [HttpPost]        
        [Route("accept")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public IActionResult Index()
        {
            var email = this.HttpContext.User.Identity.Name;
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new InvalidOperationException($"Accept terms attempted on unauthorised user");
            }

            var user = context.GetMcUsers(email).SingleOrDefault();
            if (user == null)
            {
                return NotFound();
            }

            user.AcceptTermsDateUtc = DateTime.UtcNow;
            
            context.Save();

            return Ok();
        } 
    }
}