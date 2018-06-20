using Microsoft.AspNetCore.Builder;

namespace GovUk.Education.ManageCourses.Api.Middleware
{
    public static class BearerTokenExtensions
    {
        public static IApplicationBuilder UseBearerTokenAuthorisation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BearerTokenAuthorisation>();
        }
    }
}
