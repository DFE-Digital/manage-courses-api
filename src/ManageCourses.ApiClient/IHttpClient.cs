using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.ApiClient
{
    public interface IHttpClient : IManageCoursesApiClientConfiguration
    {
        Task<HttpResponseMessage> GetAsync(Uri queryUri);
        Task<HttpResponseMessage> PostAsync(Uri queryUri, StringContent content);
    }
}
