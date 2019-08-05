using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Services.Publish;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    public class ManageCoursesBackendServiceTests
    {
        private Mock<IManageCoursesBackendJwtService> _manageCoursesBackendJwtService;
        private IManageCoursesBackendService _manageCoursesBackendService;

        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HttpClient _mockHttp;


        [SetUp]
        public void SetUp()
        {
            _manageCoursesBackendJwtService = new Mock<IManageCoursesBackendJwtService>();
            _manageCoursesBackendJwtService.Setup(x => x.GetCurrentUserToken()).Returns("GetCurrentUserToken");

            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            _mockHttp = new HttpClient(_mockHttpMessageHandler.Object);
            _mockHttp.BaseAddress = new Uri("http://mocked.com");
            _manageCoursesBackendService = new ManageCoursesBackendService(_mockHttp, _manageCoursesBackendJwtService.Object);
        }

        [Test]
        public async Task SaveCourse()
        {
            var providerCode = "providerCode";
            var courseCode = "courseCode";
            var email = "email";
            var postUrl = $"/api/v2/providers/{providerCode}/courses/{courseCode}/publish";

             _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                    x.Headers.Authorization.Scheme == "Bearer" &&
                    x.Headers.Authorization.Parameter == "GetCurrentUserToken" &&
                    x.RequestUri.AbsolutePath == postUrl), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.OK
                });

            var act = await _manageCoursesBackendService.SaveCourse(providerCode, courseCode, email);

            _manageCoursesBackendJwtService.Verify(x => x.GetCurrentUserToken(), Times.AtMostOnce());
            _mockHttpMessageHandler.Protected().Verify("SendAsync", Times.AtMostOnce(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());

            act.Should().BeTrue();
        }

        [Test]
        public async Task SaveCourses()
        {
            var providerCode = "providerCode";
            var email = "email";
            var postUrl = $"/api/v2/providers/{providerCode}/publish";

             _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                    x.Headers.Authorization.Scheme == "Bearer" &&
                    x.Headers.Authorization.Parameter == "GetCurrentUserToken" &&
                    x.RequestUri.AbsolutePath == postUrl), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.OK
                });

            var act = await _manageCoursesBackendService.SaveCourses(providerCode, email);

            _manageCoursesBackendJwtService.Verify(x => x.GetCurrentUserToken(), Times.AtMostOnce());
            _mockHttpMessageHandler.Protected().Verify("SendAsync", Times.AtMostOnce(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());

            act.Should().BeTrue();
        }
    }
}
