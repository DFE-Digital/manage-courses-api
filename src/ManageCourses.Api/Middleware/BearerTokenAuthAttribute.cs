using Microsoft.AspNetCore.Authorization;

namespace GovUk.Education.ManageCourses.Api.Middleware
{
    public class BearerTokenAuthAttribute : AuthorizeAttribute
    {
        public BearerTokenAuthAttribute()
        {
            AuthenticationSchemes = BearerTokenDefaults.AuthenticationScheme;
        }
    }
}
