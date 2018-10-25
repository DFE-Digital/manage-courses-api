using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;

namespace GovUk.Education.ManageCourses.Tests.SmokeTests
{
    public partial class ApiEndpointTests
    {
        private class MockApiClientConfiguration : IManageCoursesApiClientConfiguration
        {
            private readonly string _accessToken;
            private readonly string _baseUrl;

            public MockApiClientConfiguration(string accessToken, string baseUrl)
            {
                _accessToken = accessToken;
                _baseUrl = baseUrl;
            }

            public Task<string> GetAccessTokenAsync()
            {
                return Task.FromResult(_accessToken);
            }

            public string GetBaseUrl() => _baseUrl;
        }
    }
}
