using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using GovUk.Education.ManageCourses.Api.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GovUk.Education.ManageCourses.ApiClient
{
    public class ManageCoursesApiClient
    {
        private readonly IHttpClient _httpClient;
        private readonly string _apiUri;

        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public ManageCoursesApiClient(IManageCoursesApiClientConfiguration apiClientConfiguration, HttpClient httpClient)
        {
            var apiUri = apiClientConfiguration.GetBaseUrl();
            if(string.IsNullOrWhiteSpace(apiUri))
            {
                throw new ManageCoursesApiException($"Failed to instantiate due apiUri is null or white space");
            }

            var accessToken = apiClientConfiguration.GetAccessTokenAsync().Result;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient = new HttpClientWrapper(httpClient);
            _apiUri = apiUri;
            if (_apiUri.EndsWith('/')) { _apiUri = _apiUri.Remove(_apiUri.Length - 1); }
        }

        private async Task PostObjects(string apiPath, object payload, NameValueCollection nameValueCollection = null)
        {
            var uri = GetUri($"{apiPath}", nameValueCollection);
            await PostObjects(uri, payload);
        }

        private async Task<T> PostObjects<T>(string apiPath, object payload, NameValueCollection nameValueCollection = null)
        {
            T objects = default(T);
            var uri = GetUri($"{apiPath}", nameValueCollection);
            var response = await PostObjects(uri, payload);
             if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                objects = JsonConvert.DeserializeObject<T>(jsonResponse);
            }

            return objects;
        }

        private async Task<HttpResponseMessage> PostObjects(Uri queryUri, object payload)
        {
            var payloadJson = payload != null ? JsonConvert.SerializeObject(payload, _serializerSettings) : string.Empty;
            var payloadStringContent = new StringContent(payloadJson, Encoding.UTF8, "application/json" );

            var response = await _httpClient.PostAsync(queryUri, payloadStringContent);
            return response;
        }

        private async Task<T> GetObjects<T>(string apiPath)
        {
            var uri = GetUri(apiPath);

            return await GetObjects<T>(uri);
        }

        private async Task<T> GetObjects<T>(Uri queryUri)
        {
            T objects = default(T);
            var response = await _httpClient.GetAsync(queryUri);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                objects = JsonConvert.DeserializeObject<T>(jsonResponse);
            }

            return objects;
        }

        private Uri GetUri(string apiPath, NameValueCollection nameValueCollection = null)
        {
            var uri = new Uri(_apiUri);
            var builder = new UriBuilder(uri);
            if (!builder.Path.EndsWith('/') && !apiPath.StartsWith('/')) { builder.Path += '/'; }
            else if (builder.Path.EndsWith('/') && apiPath.StartsWith('/')) { apiPath = apiPath.Substring(1); }
            builder.Path += apiPath;
            var queryString = nameValueCollection != null ? nameValueCollection.ToString() : string.Empty;
            builder.Query = queryString;
            return builder.Uri;
        }

        public async Task<UcasInstitutionEnrichmentGetModel> Enrichment_GetInstitutionAsync(string ucasInstitutionCode)
        {
            return await GetObjects<UcasInstitutionEnrichmentGetModel>($"enrichment/institution/{ucasInstitutionCode}");
        }
        public async Task Enrichment_SaveInstitutionAsync(string ucasInstitutionCode, UcasInstitutionEnrichmentPostModel model)
        {
            await PostObjects($"enrichment/institution/{ucasInstitutionCode}", model);
        }
        public async Task<UcasCourseEnrichmentGetModel> Enrichment_GetCourseAsync(string ucasInstitutionCode, string ucasCourseCode)
        {
            return await GetObjects<UcasCourseEnrichmentGetModel>($"enrichment/institution/{ucasInstitutionCode}/course/{ucasCourseCode}");
        }
        public async Task Enrichment_SaveCourseAsync(string ucasInstitutionCode, string ucasCourseCode, CourseEnrichmentModel model)
        {
            await PostObjects($"enrichment/institution/{ucasInstitutionCode}/course/{ucasCourseCode}", model);
        }

        public async Task Invite_IndexAsync(string email)
        {
            var nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
            nameValueCollection[nameof(email)] = email;

            await PostObjects($"invite", null, nameValueCollection);
        }


        public async Task<bool> Publish_PublishCoursesToSearchAndCompareAsync(string instCode)
        {
            return await PostObjects<bool>($"publish/organisation/{instCode}", null);
        }
        public async Task<SearchAndCompare.Domain.Models.Course> Publish_GetSearchAndCompareCourseAsync(string instCode, string courseCode)
        {
            return await GetObjects<SearchAndCompare.Domain.Models.Course>($"publish/searchandcompare/{instCode}/{courseCode}");
        }

        // No coverage
        public Task AcceptTerms_IndexAsync()
        {
            return Task.CompletedTask;
        }
        public Task AccessRequest_IndexAsync(AccessRequest request)
        {
            return Task.CompletedTask;
        }

        public Task<Domain.Models.Course> Courses_GetAsync(string instCode, string ucasCode)
        {
            return null;
        }
        public Task<InstitutionCourses> Courses_GetAllAsync(string instCode)
        {
            return null;
        }

        // No coverage
        public Task<UserOrganisation> Organisations_GetAsync(string instCode)
        {
            return null;
        }

        public Task<System.Collections.ObjectModel.ObservableCollection<UserOrganisation>> Organisations_GetAllAsync()
        {
            return null;
        }
        public Task<bool> Publish_PublishCourseToSearchAndCompareAsync(string instCode, string courseCode)
        {
            return null;
        }
    }
}