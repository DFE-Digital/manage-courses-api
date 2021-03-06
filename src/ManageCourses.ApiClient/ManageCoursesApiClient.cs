﻿using GovUk.Education.ManageCourses.Api.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GovUk.Education.ManageCourses.ApiClient
{
    public class ManageCoursesApiClient
    {
        private readonly IHttpClient _httpClient;
        private readonly string _baseUrl;

        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public ManageCoursesApiClient(string apiUrl, IHttpClient httpClient)
        {
            if(string.IsNullOrWhiteSpace(apiUrl))
            {
                throw new ManageCoursesApiException($"Failed to instantiate due apiUrl is null or white space");
            }

            if(httpClient == null)
            {
                throw new ManageCoursesApiException($"Failed to instantiate due to httpClient is null");
            }

            if (apiUrl.EndsWith('/')) { apiUrl = apiUrl.Remove(apiUrl.Length - 1); }
            _baseUrl = $"{apiUrl}/api";
            _httpClient = httpClient;
        }

        private async Task PostObjects(string apiPath, object payload, NameValueCollection nameValueCollection = null)
        {
            var uri = GetUri($"{apiPath}", nameValueCollection);
            await PostObjects(uri, payload);
        }

        protected async Task<T> PostObjects<T>(string apiPath, object payload, NameValueCollection nameValueCollection = null)
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
            var uri = new Uri(_baseUrl);
            var builder = new UriBuilder(uri);
            if (!builder.Path.EndsWith('/') && !apiPath.StartsWith('/')) { builder.Path += '/'; }
            else if (builder.Path.EndsWith('/') && apiPath.StartsWith('/')) { apiPath = apiPath.Substring(1); }
            builder.Path += apiPath;
            var queryString = nameValueCollection != null ? nameValueCollection.ToString() : string.Empty;
            builder.Query = queryString;
            return builder.Uri;
        }

        public async Task<UcasProviderEnrichmentGetModel> Enrichment_GetProviderAsync(string ucasProviderCode)
        {
            return await GetObjects<UcasProviderEnrichmentGetModel>($"enrichment/provider/{ucasProviderCode}");
        }
        public async Task Enrichment_SaveProviderAsync(string ucasProviderCode, UcasProviderEnrichmentPostModel model)
        {
            await PostObjects($"enrichment/provider/{ucasProviderCode}", model);
        }
        public async Task<UcasCourseEnrichmentGetModel> Enrichment_GetCourseAsync(string ucasProviderCode, string ucasCourseCode)
        {
            return await GetObjects<UcasCourseEnrichmentGetModel>($"enrichment/provider/{ucasProviderCode}/course/{ucasCourseCode}");
        }
        public async Task Enrichment_SaveCourseAsync(string ucasProviderCode, string ucasCourseCode, CourseEnrichmentModel model)
        {
            await PostObjects($"enrichment/provider/{ucasProviderCode}/course/{ucasCourseCode}", model);
        }

        public async Task Invite_IndexAsync(string email)
        {
            var nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
            nameValueCollection[nameof(email)] = email;

            await PostObjects($"invite", null, nameValueCollection);
        }

        public async Task<bool> Publish_PublishCoursesToSearchAndCompareAsync(string providerCode)
        {
            return await PostObjects<bool>($"publish/organisation/{providerCode}", null);
        }
        public async Task<SearchAndCompare.Domain.Models.Course> Publish_GetSearchAndCompareCourseAsync(string providerCode, string courseCode)
        {
            return await GetObjects<SearchAndCompare.Domain.Models.Course>($"publish/searchandcompare/{providerCode}/{courseCode}");
        }

        public async Task AcceptTerms_IndexAsync()
        {
            await PostObjects($"acceptterms/accept", null);
        }
        public async Task AccessRequest_IndexAsync(AccessRequest request)
        {
            await PostObjects($"accessrequest", request);
        }

        public async Task<Domain.Models.Course> Courses_GetAsync(string providerCode, string ucasCode)
        {
            return await GetObjects<Domain.Models.Course>($"courses/{providerCode}/course/{ucasCode}");
        }
        public async Task<List<Domain.Models.Course>> Courses_GetAllAsync(string providerCode)
        {
            return await GetObjects<List<Domain.Models.Course>>($"courses/{providerCode}");
        }

        public async Task<ProviderSummary> Organisations_GetAsync(string providerCode)
        {
            return await GetObjects<ProviderSummary>($"organisations/{providerCode}");
        }

        public async Task<IEnumerable<ProviderSummary>> Organisations_GetAllAsync()
        {
            return await GetObjects<IEnumerable<ProviderSummary>>($"organisations");
        }
        public async Task<bool> Publish_PublishCourseToSearchAndCompareAsync(string providerCode, string courseCode)
        {
            return await PostObjects<bool>($"publish/course/{providerCode}/{courseCode}", null);
        }
    }
}
