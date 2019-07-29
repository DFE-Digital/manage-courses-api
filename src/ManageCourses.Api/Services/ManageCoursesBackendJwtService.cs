using GovUk.Education.ManageCourses.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace GovUk.Education.ManageCourses.Api.Services
{
    public class ManageCoursesBackendJwtService : IManageCoursesBackendJwtService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SigningCredentials _signingCredentials;
        public ManageCoursesBackendJwtService(IHttpContextAccessor httpContextAccessor, McConfig mcConfig)
        {
            _httpContextAccessor = httpContextAccessor;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(mcConfig.ManageCoursesBackendKey));
            _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }

        private ClaimsIdentity GetIdentity() => (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity) ?? new ClaimsIdentity();

        private IEnumerable<Claim> GetClaims() => GetIdentity()
            .FindAll(x => x.Type == ClaimTypes.NameIdentifier || x.Type == ClaimTypes.Email)
            .Select(x =>
                    {
                        var claimType = x.Type == ClaimTypes.NameIdentifier ? "sign_in_user_id" : "email";

                        return new Claim(claimType, x.Value);
                    }
                );

        private bool IsBearerTokenType() => BearerTokenDefaults.AuthenticationScheme.Equals(GetIdentity().AuthenticationType) && GetIdentity().IsAuthenticated;

        public string GetCurrentUserToken()
        {
            var result = "";

            var claims = GetClaims();
            var isValid = IsBearerTokenType() &&
                claims != null &&
                claims.Count() == 2 &&
                !claims.All(x => string.IsNullOrWhiteSpace(x.Value));

            if (isValid)
            {
                var tokenDescriptor = new JwtSecurityToken(
                    claims: claims,
                    signingCredentials: _signingCredentials);

                result = (new JwtSecurityTokenHandler()).WriteToken(tokenDescriptor);
            }

            return result;
        }
    }
}
