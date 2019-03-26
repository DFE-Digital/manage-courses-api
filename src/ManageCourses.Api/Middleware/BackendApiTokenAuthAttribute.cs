using Microsoft.AspNetCore.Authorization;

namespace GovUk.Education.ManageCourses.Api.Middleware
{
    public class BackendApiTokenAuthAttribute : AuthorizeAttribute
    {
        public BackendApiTokenAuthAttribute()
        {
            AuthenticationSchemes = BackendBearerTokenApiKeyDefaults.AuthenticationScheme;
        }
    }
}
