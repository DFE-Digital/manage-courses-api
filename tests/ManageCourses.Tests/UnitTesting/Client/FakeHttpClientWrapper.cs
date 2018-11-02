using System.Net.Http;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting.Client
{
    public class FakeHttpClientWrapper : HttpClientWrapper
    {
        private readonly string _accessToken;

        public FakeHttpClientWrapper(string accessToken, HttpClient httpClient) : base(httpClient)
        {
            _accessToken = accessToken;
        }

        public override string GetAccessToken()
        {
            return this._accessToken;
        }
    }
}
