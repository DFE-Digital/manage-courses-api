using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text.Encodings.Web;
using GovUk.Education.ManageCourses.Api.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GovUk.Education.ManageCourses.Api.Services;

namespace GovUk.Education.ManageCourses.Api.Middleware
{
    public class BearerTokenHandler : AuthenticationHandler<BearerTokenOptions>
    {
        private readonly HttpClient _backChannel;
        private readonly IManageCoursesDbContext _manageCoursesDbContext;

        private readonly IUserService _userService;
        private ILogger<BearerTokenHandler> _logger;

        public BearerTokenHandler(IOptionsMonitor<BearerTokenOptions> options, IManageCoursesDbContext manageCoursesDbContext, IUserService userService, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _manageCoursesDbContext = manageCoursesDbContext;
            _backChannel = new HttpClient();
            _userService = userService;
            _logger = logger.CreateLogger<BearerTokenHandler>();
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var accessToken = Request.GetAccessToken();

            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogDebug("Bearer token not found in request headers");
                return AuthenticateResult.NoResult();
            }

            try
            {
                var userDetails = GetJsonUserDetails(accessToken);
                try
                {
                    await _userService.UserSignedInAsync(userDetails);
                }
                catch (McUserNotFoundException)
                {
                    _logger.LogWarning($"SignIn subject {userDetails.Subject} not found in McUsers data");
                    return AuthenticateResult.NoResult();
                }

                var identity = new ClaimsIdentity(
                    new[] {
                        new Claim (ClaimTypes.NameIdentifier, userDetails.Subject),
                        new Claim (ClaimTypes.Email, userDetails.Email)
                    }, BearerTokenDefaults.AuthenticationScheme, ClaimTypes.Email, null);

                var princical = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(princical, BearerTokenDefaults.AuthenticationScheme);
                _logger.LogDebug("User successfully signed in. SignIn-Id {0}", userDetails.Subject);
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex);
            }
        }

        private JsonUserDetails GetJsonUserDetails(string accessToken)
        {
            var responsesString = "";

            using (var request = new HttpRequestMessage())
            {
                var uri = Options.UserinfoEndpoint;
                request.RequestUri = new Uri(uri);
                request.Method = new HttpMethod("GET");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = _backChannel.SendAsync(request).Result;

                response.EnsureSuccessStatusCode();

                responsesString = response.Content.ReadAsStringAsync().Result;
            }

            var userDetails = JsonConvert.DeserializeObject<JsonUserDetails>(responsesString, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            });

            return userDetails;
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var authResult = HandleAuthenticateOnceSafeAsync().Result;
            if (!authResult.Succeeded && authResult.Failure != null)
            {
                _logger.LogDebug(authResult.Failure, "Failed challenge");
                Context.Response.StatusCode = 404;
                return Task.CompletedTask;
            }

            return base.HandleChallengeAsync(properties);
        }
    }
}
