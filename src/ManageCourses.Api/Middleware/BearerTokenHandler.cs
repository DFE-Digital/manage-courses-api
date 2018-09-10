using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Exceptions;
using GovUk.Education.ManageCourses.Api.Services.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace GovUk.Education.ManageCourses.Api.Middleware
{
    public class BearerTokenHandler : AuthenticationHandler<BearerTokenOptions>
    {
        private readonly HttpClient _backChannel;

        private readonly IUserService _userService;
        private readonly ILogger<BearerTokenHandler> _logger;

        public BearerTokenHandler(IOptionsMonitor<BearerTokenOptions> options, IUserService userService, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _backChannel = new HttpClient();
            _userService = userService;
            _logger = logger.CreateLogger<BearerTokenHandler>();
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                var accessToken = Request.GetAccessToken();

                if (string.IsNullOrEmpty(accessToken))
                {
                    _logger.LogDebug("Bearer token not found in request headers");
                    return AuthenticateResult.NoResult();
                }

                var mcUser = await _userService.GetFromCacheAsync(accessToken);
                var cacheMiss = mcUser == null;
                if (cacheMiss)
                {
                    var userDetails = await GetDetailsFromOAuthAsync(accessToken);
                    try
                    {
                        mcUser = await _userService.LoginAsync(userDetails);
                    }
                    catch (McUserNotFoundException ex)
                    {
                        _logger.LogWarning($"SignIn subject {userDetails.Subject} not found in McUsers data");
                        return AuthenticateResult.Fail(ex);
                    }
                    await _userService.CacheTokenAsync(accessToken, mcUser);
                }
                await _userService.LoggedInAsync(mcUser);

                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, mcUser.SignInUserId),
                    new Claim(ClaimTypes.Email, mcUser.Email)
                }, BearerTokenDefaults.AuthenticationScheme, ClaimTypes.Email, null);

                var princical = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(princical, BearerTokenDefaults.AuthenticationScheme);
                _logger.LogDebug("User successfully signed in. SignIn-Id {0}", mcUser.SignInUserId);
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex);
            }
        }

        private async Task<JsonUserDetails> GetDetailsFromOAuthAsync(string accessToken)
        {
            string responsesString;

            using (var request = new HttpRequestMessage())
            {
                var uri = Options.UserinfoEndpoint;
                request.RequestUri = new Uri(uri);
                request.Method = new HttpMethod("GET");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await _backChannel.SendAsync(request);

                response.EnsureSuccessStatusCode();

                responsesString = await response.Content.ReadAsStringAsync();
            }

            var userDetails = JsonConvert.DeserializeObject<JsonUserDetails>(responsesString, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            });

            return userDetails;
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            // this method is pretend-async because it's an override!!

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
