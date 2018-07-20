using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GovUk.Education.ManageCourses.Api.Services;

namespace GovUk.Education.ManageCourses.Api.Middleware
{
    public class BearerTokenApiKeyHandler : AuthenticationHandler<BearerTokenApiKeyOptions>
    {

        private readonly RequestDelegate _next;

        private readonly HttpClient _backChannel;
        private readonly IManageCoursesDbContext _manageCoursesDbContext;

        private readonly IUserLogService _userLogService;

        public BearerTokenApiKeyHandler(IOptionsMonitor<BearerTokenApiKeyOptions> options, IManageCoursesDbContext manageCoursesDbContext, IUserLogService userLogService, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            this._manageCoursesDbContext = manageCoursesDbContext;
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

