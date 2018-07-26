using System;
using Microsoft.AspNetCore.Authentication;

namespace GovUk.Education.ManageCourses.Api.Middleware
{
    public static class BearerTokenExtensions
    {
        public static AuthenticationBuilder AddBearerToken(this AuthenticationBuilder builder, Action<BearerTokenOptions> bearerTokenOptions)
        {
            return builder.AddScheme<BearerTokenOptions, BearerTokenHandler>(BearerTokenDefaults.AuthenticationScheme, BearerTokenDefaults.AuthenticationDisplayName, bearerTokenOptions);
        }
    }
}
