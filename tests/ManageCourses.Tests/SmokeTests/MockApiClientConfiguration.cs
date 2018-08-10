using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;

namespace GovUk.Education.ManageCourses.Tests.SmokeTests
{
    public partial class ApiEndpointTests
    {
        private class MockApiClientConfiguration : IManageCoursesApiClientConfiguration
        {
            private readonly string _accessToken;

            public MockApiClientConfiguration(string accessToken)
            {
                _accessToken = accessToken;
            }

            public Task<string> GetAccessTokenAsync()
            {
                return Task.FromResult(_accessToken);
            }
        }
    }
}
