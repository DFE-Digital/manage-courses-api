using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace GovUk.Education.ManageCourses.Tests.Helpers
{
    public static class ControllerTestsHelper
    {
        public static void SetControllerContext(this Controller controller, string name)
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> {new Claim(ClaimTypes.NameIdentifier, name)}, "test", ClaimTypes.NameIdentifier.ToString(), ""))
                },
            };
        }
    }
}