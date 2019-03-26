using System;
using Microsoft.AspNetCore.Authentication;

namespace GovUk.Education.ManageCourses.Api.Middleware
{
    public static class BackendBearerTokenApiKeyExtensions
    {
        public static AuthenticationBuilder AddBackendBearerTokenApiKey(this AuthenticationBuilder builder, Action<BearerTokenApiKeyOptions> bearerTokenApiKeyOptions)
        {
            return builder.AddScheme<BearerTokenApiKeyOptions, BearerTokenApiKeyHandler>(BackendBearerTokenApiKeyDefaults.AuthenticationScheme, BackendBearerTokenApiKeyDefaults.AuthenticationDisplayName, bearerTokenApiKeyOptions);
        }
    }
}
