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
using GovUk.Education.ManageCourses.Api.Services.Users;
using Microsoft.EntityFrameworkCore;

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

            SubjectAndEmail subjectAndEmail = null;
            try
            {
                subjectAndEmail = GetSubjectAndEmailFromDatabase(accessToken);
                if (subjectAndEmail == null)
                {
                    var userDetails = GetJsonUserDetailsFromOauth(accessToken);
                    subjectAndEmail = new SubjectAndEmail(userDetails.Subject, userDetails.Email);
                    await _userService.UserSignedInAsync(accessToken, userDetails);
                }
            }               
            catch (McUserNotFoundException)
            {
                _logger.LogWarning($"SignIn subject {subjectAndEmail?.Subject} not found in McUsers data");
                return AuthenticateResult.NoResult();
            }            
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex);
            }
            
            var identity = new ClaimsIdentity(
                new[] {
                    new Claim (ClaimTypes.NameIdentifier, subjectAndEmail.Subject),
                    new Claim (ClaimTypes.Email, subjectAndEmail.Email)
                }, BearerTokenDefaults.AuthenticationScheme, ClaimTypes.Email, null);

            var princical = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(princical, BearerTokenDefaults.AuthenticationScheme);
            _logger.LogDebug("User successfully signed in. SignIn-Id {0}", subjectAndEmail.Subject);
            return AuthenticateResult.Success(ticket);            
        }

        private SubjectAndEmail GetSubjectAndEmailFromDatabase(string accessToken)
        {
            var dateCutoff = DateTime.UtcNow.AddMinutes(-30);
            var session = _manageCoursesDbContext.McSessions
                .Include(x => x.McUser)
                .Where(x => x.AccessToken == accessToken && x.CreatedUtc > dateCutoff)
                .FirstOrDefault(); /*   There is an edge case where more than one record for a valid session 
                                        could be added, e.g. when clock skew occurs between two authentications. 
                                        Any of these overlapping records would qualify and the redundancy is quite harmless. 
                                        So we use First, not Single, here. */

            if (session == null)
            {
                return null;
            }
            
            return new SubjectAndEmail(session.McUser.SignInUserId, session.McUser.Email);
        }

        private JsonUserDetails GetJsonUserDetailsFromOauth(string accessToken)
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

        private class SubjectAndEmail
        {
            public string Subject {get; private set;}
            public string Email {get; private set;}

            public SubjectAndEmail(string subject, string email)
            {
                Subject = subject;
                Email = email;
            }
        }
    }
}
