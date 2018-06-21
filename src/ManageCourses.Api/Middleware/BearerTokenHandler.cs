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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GovUk.Education.ManageCourses.Api.Middleware
{
    public class BearerTokenHandler : AuthenticationHandler<BearerTokenOptions>
    {

        private readonly RequestDelegate _next;

        private readonly HttpClient _backChannel;
        private readonly IManageCoursesDbContext _manageCoursesDbContext;

        public BearerTokenHandler(IOptionsMonitor<BearerTokenOptions> options, IManageCoursesDbContext manageCoursesDbContext, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            this._manageCoursesDbContext = manageCoursesDbContext;
            this._backChannel = new HttpClient();
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var accessToken = GetAccessToken();

            if (!string.IsNullOrEmpty(accessToken))
            {
                try
                {
                    var userDetails = GetJsonUserDetails(accessToken);

                    var mcuser = _manageCoursesDbContext.McUsers.First(x => x.Email == userDetails.Email);

                    var identity = new ClaimsIdentity( 
                        new[] {
                            new Claim (ClaimTypes.NameIdentifier, userDetails.Subject),
                            new Claim (ClaimTypes.Email, userDetails.Email)
                        }, BearerTokenDefaults.AuthenticationScheme, ClaimTypes.Email, null);
                    
                    var princical = new ClaimsPrincipal(identity);
                    
                    var ticket = new AuthenticationTicket(princical, BearerTokenDefaults.AuthenticationScheme);

                    return AuthenticateResult.Success(ticket);
                }
                catch (Exception ex)
                {
                    return AuthenticateResult.Fail(ex);
                }
            }
            else
            {
                return AuthenticateResult.NoResult();
            }
        }

        private string GetAccessToken()
        {
            var authorizationHeaderValues = Request.Headers.ContainsKey("Authorization") ?
                ((string)Request.Headers["Authorization"]).Split(' ') : new[] { "", "" };

            var accessToken = authorizationHeaderValues.Length == 2 && authorizationHeaderValues[0].ToLowerInvariant().Equals("bearer") ?
                authorizationHeaderValues[1] : "";

            return accessToken;
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

