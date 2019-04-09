using System.Net.Http;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Tests.DbIntegration;
using GovUk.Education.ManageCourses.Tests.TestUtilities;
using GovUk.Education.ManageCourses.Tests.UnitTesting.Client;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.SmokeTests
{
    public abstract class ApiSmokeTestBase : DbIntegrationTestBase
    {
        protected ApiLocalWebHost LocalWebHost;

        public override void OneTimeSetup()
        {
            LocalWebHost = new ApiLocalWebHost(Config).Launch();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            LocalWebHost?.Stop();
        }

        /// <summary>
        /// Backend doesn't use the api client, but it's useful for testing so this class extends the client with methods called backend.
        /// </summary>
        protected class BackendManageCoursesApiClient : ManageCoursesApiClient
        {
            public BackendManageCoursesApiClient(string apiUrl, IHttpClient httpClient) :base(apiUrl, httpClient)
            {
            }

            internal async Task<ResponseResult> Internal_Publish_PublishCourseToSearchAndCompareAsync(string providerCode, string courseCode, BackendRequest request)
            {
                return await PostObjects<ResponseResult>($"publish/internal/course/{providerCode}/{courseCode}", request);
            }

            internal async Task<ResponseResult> Internal_Publish_PublishCoursesToSearchAndCompareAsync(string providerCode, BackendRequest request)
            {
                return await PostObjects<ResponseResult>($"publish/internal/courses/{providerCode}", request);
            }
        }

        protected BackendManageCoursesApiClient BuildClient(string accessToken)
        {
            var httpClient = new HttpClient();
            var httpClientWrapper = new FakeHttpClientWrapper(accessToken, httpClient);
            return new BackendManageCoursesApiClient(LocalWebHost.Address, httpClientWrapper);
        }

        internal class ResponseResult
        {
            public bool Result {get; set;}
        }
    }
}
