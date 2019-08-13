using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.ApiClient;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;


namespace GovUk.Education.ManageCourses.Tests.SmokeTests
{
    /// <summary>
    /// These tests poke the http endpoints of the api whilst the api is running in a
    /// a captive web host.
    /// </summary>
    [TestFixture]
    [Category("Smoke")]
    public class ApiBackendPublishCoursesTests : ApiSmokeTestBase
    {
        private const string Email = "feddie.krueger@example.org";
        private const string ProviderCode = "providerCode";

        /// <summary>
        ///     Tests a valid http status 200 with result as false, true cannot be tested in this form as it will also make an external call.
        /// </summary>
        [Test]
        public async Task Internal_Publish_PublishCourseToSearchAndCompareAsync()
        {
            var client = BuildClient(TestConfig.BackendApiKey);

            var result = await client.Internal_Publish_PublishCoursesToSearchAndCompareAsync(
                ProviderCode, new BackendRequest());

            result.Result.Should().Be(false);
        }

        [Test]
        public void Internal_Publish_PublishCourseToSearchAndCompareAsync_internalServerError_500()
        {
            var client = BuildClient(TestConfig.BackendApiKey);

            Func<Task> act = async () =>
                await client.Internal_Publish_PublishCoursesToSearchAndCompareAsync(
                    ProviderCode, new BackendRequest{Email = Email});

            act.Should().Throw<ManageCoursesApiException>()
                .WithMessage($"API POST Failed uri {LocalWebHost.Address}/api/publish/internal/courses/{ProviderCode}")
                .Which.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Test]
        public void Internal_Publish_PublishCourseToSearchAndCompareAsync_badAccesCode_404()
        {
            var client = BuildClient("badAccesCode");

            Func<Task> act = async () =>
                await client.Internal_Publish_PublishCoursesToSearchAndCompareAsync(
                    ProviderCode, new BackendRequest{Email = Email});

            act.Should().Throw<ManageCoursesApiException>()
                .WithMessage($"API POST Failed uri {LocalWebHost.Address}/api/publish/internal/courses/{ProviderCode}")
                .Which.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public void Internal_Publish_PublishCourseToSearchAndCompareAsync_noAccesCode_401()
        {
            var client = BuildClient("");

            Func<Task> act = async () =>
                await client.Internal_Publish_PublishCoursesToSearchAndCompareAsync(
                    ProviderCode, new BackendRequest{Email = Email});

            act.Should().Throw<ManageCoursesApiException>()
                .WithMessage($"API POST Failed uri {LocalWebHost.Address}/api/publish/internal/courses/{ProviderCode}")
                .Which.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
