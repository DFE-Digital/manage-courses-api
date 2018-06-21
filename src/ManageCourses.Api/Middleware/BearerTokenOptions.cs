using Microsoft.AspNetCore.Authentication;

namespace GovUk.Education.ManageCourses.Api.Middleware
{
    public class BearerTokenOptions : AuthenticationSchemeOptions    
    {
        public string UserinfoEndpoint { get; set; }
    }
}
