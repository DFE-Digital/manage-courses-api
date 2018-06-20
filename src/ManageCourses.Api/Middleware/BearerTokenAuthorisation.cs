using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace GovUk.Education.ManageCourses.Api.Middleware {
    public class BearerTokenAuthorisation {

        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        
        private readonly HttpClient _backChannel;

        public BearerTokenAuthorisation (RequestDelegate next, IConfiguration config) {
            this._next = next;
            this._config = config;

            this._backChannel = new HttpClient ();
        }

        public Task Invoke(HttpContext context)
        {
            var authorizationHeaderValues = context.Request.Headers.ContainsKey("Authorization") ?
                ((string)context.Request.Headers["Authorization"]).Split(' ') : new[] { "", "" };

            var accessToken = authorizationHeaderValues.Length == 2 && authorizationHeaderValues[0].ToLowerInvariant().Equals("bearer") ?
                authorizationHeaderValues[1] : "";

            try
            {
                if (!string.IsNullOrEmpty(accessToken))
                {
                    var responsesString = "";

                    using (var request = new HttpRequestMessage())
                    {
                        var uri = _config["auth:oidc:userinfo_endpoint"];
                        request.RequestUri = new Uri(uri);
                        request.Method = new HttpMethod("GET");
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                        var response = _backChannel.SendAsync(request).Result;
                        response.EnsureSuccessStatusCode();

                        responsesString = response.Content.ReadAsStringAsync().Result;
                    }

                    if (!string.IsNullOrEmpty(responsesString))
                    {
                        var userDetails = JsonConvert.DeserializeObject<JsonUserDetails>(responsesString, new JsonSerializerSettings
                        {
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                        });

                        var identity = new ClaimsIdentity(new[] {
                             new Claim (ClaimTypes.NameIdentifier, userDetails.Subject),
                                 new Claim (ClaimTypes.Email, userDetails.Email)
                        });
                        context.User.AddIdentity(identity);
                    }

                    return _next(context);
                }
                else
                {
                    context.Response.StatusCode = 401;
                    return context.Response.WriteAsync("Unauthorized");
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 404;
                return context.Response.WriteAsync("Not Found");
            }
        }
    }
}
