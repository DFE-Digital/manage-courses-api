using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.ApiClient
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> GetAsync(Uri queryUri);
        Task<HttpResponseMessage> PostAsync(Uri queryUri, StringContent content);
        // Task<HttpResponseMessage> PutAsync(Uri queryUri, StringContent content);
    }
}
