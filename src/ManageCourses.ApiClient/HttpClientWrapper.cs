using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.ApiClient
{
    public abstract class HttpClientWrapper : IHttpClient
    {
        private readonly HttpClient wrapped;

        public HttpClientWrapper(HttpClient _wrapped)
        {
            this.wrapped = _wrapped;
        }

        public virtual async Task<HttpResponseMessage> GetAsync(Uri queryUri)
        {
            var msg = $"API GET Failed uri {queryUri}";
            try
            {
                SetAccessToken();
                var result = await wrapped.GetAsync(queryUri);

                await Guard(result, msg);
                return result;
            }
            catch(ManageCoursesApiException)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new ManageCoursesApiException(msg, ex);
            }
        }

        public virtual async Task<HttpResponseMessage> PostAsync(Uri queryUri, StringContent content)
        {
            var msg = $"API POST Failed uri {queryUri}";
            try
            {
                SetAccessToken();
                var result = await wrapped.PostAsync(queryUri, content);

                await Guard(result, msg);
                return result;
            }
            catch(ManageCoursesApiException)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new ManageCoursesApiException(msg, ex);
            }
        }

        private async Task Guard(HttpResponseMessage result, string msg)
        {
            if(!result.IsSuccessStatusCode) {

                var responseData = result.Content == null ? null : await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new ManageCoursesApiException(msg, result.StatusCode, responseData);
            }
        }

        private void SetAccessToken()
        {
            var accessToken = GetAccessToken();
            wrapped.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public abstract string GetAccessToken();
    }
}
