using FluentAssertions;
using GovUk.Education.ManageCourses.Api;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Claims;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]

    public class ManageCoursesBackendJwtServiceTests
    {
        private IManageCoursesBackendJwtService _manageCoursesBackendJwtService;
        private Mock<IHttpContextAccessor> _httpContextAccessor;

        [SetUp]
        public void SetUp()
        {
            var config = new Mock<IConfiguration>();

            config.SetupGet(c => c["SETTINGS:MANAGE_BACKEND:SECRET"]).Returns("SETTINGS:MANAGE_BACKEND:SECRET");

            var _mcConfig = new McConfig(config.Object);

            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            var claims = new List<Claim>(){
                new Claim(ClaimTypes.NameIdentifier, "manage_courses_api"),
                new Claim(ClaimTypes.Email, "manage_courses@api.com")
            };

            var identity = new ClaimsIdentity(
                claims,
                BearerTokenDefaults.AuthenticationScheme
            );

            _httpContextAccessor.Setup(x => x.HttpContext.User.Identity).Returns(identity);

            _manageCoursesBackendJwtService = new ManageCoursesBackendJwtService(_httpContextAccessor.Object, _mcConfig);
        }

        [Test]
        public void GetCurrentUserToken()
        {
            var token = _manageCoursesBackendJwtService.GetCurrentUserToken();

            var expectedToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzaWduX2luX3VzZXJfaWQiOiJtYW5hZ2VfY291cnNlc19hcGkiLCJlbWFpbCI6Im1hbmFnZV9jb3Vyc2VzQGFwaS5jb20ifQ.fs0tM5Lc_5vA2w94PhlDMnq50NUn2K5SMxggdAApp_w";

            token.Should().Be(expectedToken);
        }
    }
}
