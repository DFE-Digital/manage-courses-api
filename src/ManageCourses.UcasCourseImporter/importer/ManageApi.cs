using System;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using Serilog;

namespace GovUk.Education.ManageCourses.UcasCourseImporter
{
    public class ManageApi
    {
        private readonly ILogger _logger;
        private readonly ManageCoursesApiClient _client;

        public ManageApi(ILogger logger, string apiLocation, string bearerToken)
        {
            _logger = logger;

            var patientHttpClient = new System.Net.Http.HttpClient()
            {
                Timeout = TimeSpan.FromMinutes(240)
            };

            _client = new ManageCoursesApiClient(new ApiConf(bearerToken), patientHttpClient)
            {
                BaseUrl = apiLocation
            };
        }

        public void PostPayload(UcasPayload payload)
        {
            _logger.Information("Posting to api...");
            _client.Data_ImportAsync(payload).Wait();
            _logger.Information("Done.");
        }

        private class ApiConf : IManageCoursesApiClientConfiguration
        {
            private readonly string _bearerToken;

            public ApiConf(string bearerToken)
            {
                _bearerToken = bearerToken;
            }

            public Task<string> GetAccessTokenAsync()
            {
                return Task.FromResult(_bearerToken);
            }
        }
    }
}
