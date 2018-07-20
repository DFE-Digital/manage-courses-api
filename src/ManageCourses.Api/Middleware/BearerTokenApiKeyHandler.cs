using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GovUk.Education.ManageCourses.Api.Middleware
{
    public class BearerTokenApiKeyHandler : AuthenticationHandler<BearerTokenApiKeyOptions>
    {
        public BearerTokenApiKeyHandler(IOptionsMonitor<BearerTokenApiKeyOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var accessToken = Request.GetAccessToken();

            if (!string.IsNullOrEmpty(accessToken))
            {
                if (accessToken.Equals(Options.ApiKey))
                {
                    var identity = new ClaimsIdentity(
                    new[] {
                            new Claim (ClaimTypes.NameIdentifier, "System")
                    }, BearerTokenApiKeyDefaults.AuthenticationScheme);

                    var princical = new ClaimsPrincipal(identity);

                    var ticket = new AuthenticationTicket(princical, BearerTokenApiKeyDefaults.AuthenticationScheme);

                    return AuthenticateResult.Success(ticket);
                }
                else {
                    return AuthenticateResult.Fail($"Invalid api key: {accessToken}");
                }
            }

            return AuthenticateResult.NoResult();
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var authResult = this.HandleAuthenticateOnceSafeAsync().Result;
            if (!authResult.Succeeded && authResult.Failure != null)
            {
                Logger.LogDebug(authResult.Failure, "Failed challenge");
                Context.Response.StatusCode = 404;
                return Task.CompletedTask;
            }

            return base.HandleChallengeAsync(properties);
        }
    }
}
