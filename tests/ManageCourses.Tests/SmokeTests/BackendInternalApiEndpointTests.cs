using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Tests.UnitTesting.Client;
using GovUk.Education.ManageCourses.Tests.TestUtilities;
using GovUk.Education.ManageCourses.UcasCourseImporter;
using Moq;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.Text;


namespace GovUk.Education.ManageCourses.Tests.SmokeTests
{
    /// <summary>
    /// These tests poke the http endpoints of the api whilst the api is running in a
    /// a captive web host.
    /// </summary>
    [TestFixture]
    [Category("Smoke")]
    [Explicit]
    public partial class BackendInternalApiEndpointTests : ApiSmokeTestBase
    {
        private const string Email = "feddie.krueger@example.org";

        /// <summary>
        ///     Tests a valid http status 200 with result as false, true cannot be tested in this form as it will also make an external call.
        /// </summary>
        [Test]
        public async Task Internal_Publish_PublishCourseToSearchAndCompareAsync()
        {
            var accessToken = TestConfig.BackendApiKey;

            var httpClient = new HttpClient();
            var httpClientWrapper = new FakeHttpClientWrapper(accessToken, httpClient);

            var client = new BackendManageCoursesApiClient(LocalWebHost.Address, httpClientWrapper);

            var providerCode = "providerCode";
            var courseCode = "courseCode";
            var result = await client.Internal_Publish_PublishCourseToSearchAndCompareAsync(providerCode, courseCode, new BackendRequest{Email = ""});

            result.Result.Should().Be(false);
        }

        [Test]
        public void Internal_Publish_PublishCourseToSearchAndCompareAsync_internalServerError_500()
        {
            var accessToken = TestConfig.BackendApiKey;

            var httpClient = new HttpClient();
            var httpClientWrapper = new FakeHttpClientWrapper(accessToken, httpClient);

            var client = new BackendManageCoursesApiClient(LocalWebHost.Address, httpClientWrapper);

            var providerCode = "providerCode";
            var courseCode = "courseCode";
            Func<Task> act = async () => await client.Internal_Publish_PublishCourseToSearchAndCompareAsync(providerCode, courseCode, new BackendRequest{Email = Email});

            var msg = $"API POST Failed uri {LocalWebHost.Address}/api/publish/internal/course/{providerCode}/{courseCode}";
            act.Should().Throw<ManageCoursesApiException>().WithMessage(msg).Which.StatusCode.Equals(HttpStatusCode.InternalServerError);
        }
        [Test]
        public void Internal_Publish_PublishCourseToSearchAndCompareAsync_badAccesCode_404()
        {
            var accessToken = "badAccesCode";

            var httpClient = new HttpClient();
            var httpClientWrapper = new FakeHttpClientWrapper(accessToken, httpClient);

            var client = new BackendManageCoursesApiClient(LocalWebHost.Address, httpClientWrapper);

            var providerCode = "providerCode";
            var courseCode = "courseCode";
            Func<Task> act = async () => await client.Internal_Publish_PublishCourseToSearchAndCompareAsync(providerCode, courseCode, new BackendRequest{Email = Email});

            var msg = $"API POST Failed uri {LocalWebHost.Address}/api/publish/internal/course/{providerCode}/{courseCode}";
            act.Should().Throw<ManageCoursesApiException>().WithMessage(msg).Which.StatusCode.Equals(HttpStatusCode.NotFound);
        }

        [Test]
        public void Internal_Publish_PublishCourseToSearchAndCompareAsync_noAccesCode_401()
        {
            const string accessToken = "";

            var httpClient = new HttpClient();
            var httpClientWrapper = new FakeHttpClientWrapper(accessToken, httpClient);

            var client = new BackendManageCoursesApiClient(LocalWebHost.Address, httpClientWrapper);

            var providerCode = "providerCode";
            var courseCode = "courseCode";
            Func<Task> act = async () => await client.Internal_Publish_PublishCourseToSearchAndCompareAsync(providerCode, courseCode, new BackendRequest{Email = Email});

            var msg = $"API POST Failed uri {LocalWebHost.Address}/api/publish/internal/course/{providerCode}/{courseCode}";
            act.Should().Throw<ManageCoursesApiException>().WithMessage(msg).Which.StatusCode.Equals(HttpStatusCode.Unauthorized);
        }

        private class BackendManageCoursesApiClient : ManageCoursesApiClient
        {
            public BackendManageCoursesApiClient(string apiUrl, IHttpClient httpClient) :base(apiUrl, httpClient)
            {
            }

            internal async Task<ResponseResult> Internal_Publish_PublishCourseToSearchAndCompareAsync(string providerCode, string courseCode, BackendRequest request)
            {
                return await PostObjects<ResponseResult>($"publish/internal/course/{providerCode}/{courseCode}", request);
            }
        }

        internal class ResponseResult
        {
            public bool Result {get; set;}
        }
    }
}
