using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

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
