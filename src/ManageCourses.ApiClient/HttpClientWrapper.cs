using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.ApiClient
{
    public class HttpClientWrapper : IHttpClient
    {
        private readonly HttpClient wrapped;

        public HttpClientWrapper(HttpClient wrapped)
        {
            if(wrapped == null)
            {
                throw new ManageCoursesApiException($"Failed to instantiate due to HttpClient = null");
            }

            this.wrapped = wrapped;
        }

        public async Task<HttpResponseMessage> GetAsync(Uri queryUri)
        {
            var msg = $"API GET Failed uri {queryUri}";
            try
            {
                var result = await wrapped.GetAsync(queryUri);

                await Guard(result, msg);
                return result;
            }
            catch(ManageCoursesApiException ex)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new ManageCoursesApiException(msg, ex);
            }
        }

        public async Task<HttpResponseMessage> PostAsync(Uri queryUri, StringContent content)
        {
            var msg = $"API POST Failed uri {queryUri}";
            try
            {
                var result = await wrapped.PostAsync(queryUri, content);
                await Guard(result, msg);
                return result;
            }
            catch(ManageCoursesApiException ex)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new ManageCoursesApiException(msg, ex);
            }
        }

        // public async Task<HttpResponseMessage> PutAsync(Uri queryUri, StringContent content)
        // {
        //     var msg = $"API PUT Failed uri {queryUri}";

        //     try
        //     {
        //         var result = await wrapped.PutAsync(queryUri, content);
        //         await Guard(result, msg);
        //         return result;
        //     }
        //     catch(ManageCoursesApiException ex)
        //     {
        //         throw;
        //     }
        //     catch(Exception ex)
        //     {
        //         throw new ManageCoursesApiException(msg, ex);
        //     }
        // }

        private async Task Guard(HttpResponseMessage result, string msg)
        {
            if(!result.IsSuccessStatusCode) {

                var responseData = result.Content == null ? null : await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new ManageCoursesApiException(msg, result.StatusCode, responseData);
            }
        }
    }
}
