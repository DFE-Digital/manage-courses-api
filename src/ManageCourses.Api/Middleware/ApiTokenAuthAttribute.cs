using Microsoft.AspNetCore.Authorization;

namespace GovUk.Education.ManageCourses.Api.Middleware
{
    public class ApiTokenAuthAttribute : AuthorizeAttribute
    {
        public ApiTokenAuthAttribute()
        {
            AuthenticationSchemes = BearerTokenApiKeyDefaults.AuthenticationScheme;
        }
    }
}
