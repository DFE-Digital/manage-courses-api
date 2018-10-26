using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using GovUk.Education.ManageCourses.Api.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Text;

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

        private void PostObjects(string apiPath, object payload, NameValueCollection nameValueCollection = null)
        {
            var uri = GetUri($"{apiPath}", nameValueCollection);
            PostObjects(uri, payload);
        }

        private T PostObjects<T>(string apiPath, object payload, NameValueCollection nameValueCollection = null)
        {
            T objects = default(T);
            var uri = GetUri($"{apiPath}", nameValueCollection);
            var response = PostObjects(uri, payload);
             if (response.IsSuccessStatusCode)
            {
                var jsonResponse = response.Content.ReadAsStringAsync().Result;
                objects = JsonConvert.DeserializeObject<T>(jsonResponse);
            }
            return objects;
        }

        private HttpResponseMessage PostObjects(Uri queryUri, object payload)
        {
            var payloadJson = payload != null ? JsonConvert.SerializeObject(payload, _serializerSettings) : string.Empty;
            var payloadStringContent = new StringContent(payloadJson, Encoding.UTF8, "application/json" );

            var response = _httpClient.PostAsync(queryUri, payloadStringContent).Result;
            return response;
        }

        private T GetObjects<T>(string apiPath)
        {
            var uri = GetUri(apiPath);

            return GetObjects<T>(uri);
        }

        private T GetObjects<T>(Uri queryUri)
        {
            T objects = default(T);
            var response = _httpClient.GetAsync(queryUri).Result;
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = response.Content.ReadAsStringAsync().Result;
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

    // No coverage
    // public System.Threading.Tasks.Task AcceptTerms_IndexAsync()
    // {
    //     return System.Threading.Tasks.Task.CompletedTask;
    // }
    // public System.Threading.Tasks.Task AccessRequest_IndexAsync(AccessRequest request)
    // {
    //     return System.Threading.Tasks.Task.CompletedTask;
    // }
    // public System.Threading.Tasks.Task Admin_ActionAccessRequestAsync(int accessRequestId)
    // {
    //     return System.Threading.Tasks.Task.CompletedTask;
    // }
    // public System.Threading.Tasks.Task Admin_ActionManualActionRequestAsync(string requesterEmail, string targetEmail, string firstName, string lastName)
    // {
    //     return System.Threading.Tasks.Task.CompletedTask;
    // }
    // public System.Threading.Tasks.Task<Domain.Models.Course> Courses_GetAsync(string instCode, string ucasCode)
    // {
    //     return null;
    // }
    // public System.Threading.Tasks.Task<InstitutionCourses> Courses_GetAllAsync(string instCode)
    // {
    //     return null;
    // }

    public async System.Threading.Tasks.Task<UcasInstitutionEnrichmentGetModel> Enrichment_GetInstitutionAsync(string ucasInstitutionCode)
    {
        return GetObjects<UcasInstitutionEnrichmentGetModel>($"enrichment/institution/{ucasInstitutionCode}");
    }
    public void Enrichment_SaveInstitutionAsync(string ucasInstitutionCode, UcasInstitutionEnrichmentPostModel model)
    {
        PostObjects($"enrichment/institution/{ucasInstitutionCode}", model);
    }
    public async System.Threading.Tasks.Task<UcasCourseEnrichmentGetModel> Enrichment_GetCourseAsync(string ucasInstitutionCode, string ucasCourseCode)
    {
        return GetObjects<UcasCourseEnrichmentGetModel>($"enrichment/institution/{ucasInstitutionCode}/course/{ucasCourseCode}");
    }
    public void Enrichment_SaveCourseAsync(string ucasInstitutionCode, string ucasCourseCode, CourseEnrichmentModel model)
    {
        PostObjects($"enrichment/institution/{ucasInstitutionCode}/course/{ucasCourseCode}", model);
    }

    public void Invite_IndexAsync(string email)
    {
        var nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
        nameValueCollection[nameof(email)] = email;

        PostObjects($"invite", null, nameValueCollection);
    }

    // No coverage
    // public System.Threading.Tasks.Task<UserOrganisation> Organisations_GetAsync(string instCode)
    // {
    //     return null;
    // }
    // public System.Threading.Tasks.Task<Domain.Models.Institution> Organisations_GetUcasInstitutionAsync(string instCode)
    // {
    //     return null;
    // }
    // public System.Threading.Tasks.Task<System.Collections.ObjectModel.ObservableCollection<UserOrganisation>> Organisations_GetAllAsync()
    // {
    //     return null;
    // }
    // public System.Threading.Tasks.Task<bool> Publish_PublishCourseToSearchAndCompareAsync(string instCode, string courseCode)
    // {
    //     return null;
    // }
    public async System.Threading.Tasks.Task<bool> Publish_PublishCoursesToSearchAndCompareAsync(string instCode)
    {
        return PostObjects<bool>($"publish/organisation/{instCode}", null);
    }
    public async System.Threading.Tasks.Task<SearchAndCompare.Domain.Models.Course> Publish_GetSearchAndCompareCourseAsync(string instCode, string courseCode)
    {
        return GetObjects<SearchAndCompare.Domain.Models.Course>($"publish/organisation/{instCode}/{courseCode}");
    }


    // async boilerplating

        // public async System.Threading.Tasks.Task AcceptTerms_IndexAsync(System.Threading.CancellationToken cancellationToken) {

        //     return;
        // }
        // public async System.Threading.Tasks.Task AccessRequest_IndexAsync(AccessRequest request, System.Threading.CancellationToken cancellationToken) {

        //     return;
        // }
        // public async System.Threading.Tasks.Task Admin_ActionAccessRequestAsync(int accessRequestId, System.Threading.CancellationToken cancellationToken) {

        //     return;
        // }
        // public async System.Threading.Tasks.Task Admin_ActionManualActionRequestAsync(string requesterEmail, string targetEmail, string firstName, string lastName, System.Threading.CancellationToken cancellationToken) {

        //     return;
        // }
        // public async System.Threading.Tasks.Task<Domain.Models.Course> Courses_GetAsync(string instCode, string ucasCode, System.Threading.CancellationToken cancellationToken) {

        //     return null;
        // }
        // public async System.Threading.Tasks.Task<InstitutionCourses> Courses_GetAllAsync(string instCode, System.Threading.CancellationToken cancellationToken) {

        //     return null;
        // }

        // public async System.Threading.Tasks.Task<UcasInstitutionEnrichmentGetModel> Enrichment_GetInstitutionAsync(string ucasInstitutionCode, System.Threading.CancellationToken cancellationToken) {

        //     return null;
        // }
        // public async System.Threading.Tasks.Task Enrichment_SaveInstitutionAsync(string ucasInstitutionCode, UcasInstitutionEnrichmentPostModel model, System.Threading.CancellationToken cancellationToken) {

        //     return;
        // }
        // public async System.Threading.Tasks.Task<UcasCourseEnrichmentGetModel> Enrichment_GetCourseAsync(string ucasInstitutionCode, string ucasCourseCode, System.Threading.CancellationToken cancellationToken) {

        //     return null;
        // }
        // public async System.Threading.Tasks.Task Enrichment_SaveCourseAsync(string ucasInstitutionCode, string ucasCourseCode, CourseEnrichmentModel model, System.Threading.CancellationToken cancellationToken) {

        //     return;
        // }

        // public async System.Threading.Tasks.Task Invite_IndexAsync(string email, System.Threading.CancellationToken cancellationToken) {

        //     return;
        // }
        // public async System.Threading.Tasks.Task<UserOrganisation> Organisations_GetAsync(string instCode, System.Threading.CancellationToken cancellationToken) {

        //     return null;
        // }
        // public async System.Threading.Tasks.Task<Domain.Models.Institution> Organisations_GetUcasInstitutionAsync(string instCode, System.Threading.CancellationToken cancellationToken) {

        //     return null;
        // }
        // public async System.Threading.Tasks.Task<System.Collections.ObjectModel.ObservableCollection<UserOrganisation>> Organisations_GetAllAsync(System.Threading.CancellationToken cancellationToken) {

        //     return null;
        // }
        // public async System.Threading.Tasks.Task<bool> Publish_PublishCourseToSearchAndCompareAsync(string instCode, string courseCode, System.Threading.CancellationToken cancellationToken) {

        //     return false;
        // }
        // public async System.Threading.Tasks.Task<bool> Publish_PublishCoursesToSearchAndCompareAsync(string instCode, System.Threading.CancellationToken cancellationToken) {

        //     return false;
        // }
        // public async System.Threading.Tasks.Task<Domain.Models.Course> Publish_GetSearchAndCompareCourseAsync(string instCode, string courseCode, System.Threading.CancellationToken cancellationToken) {

        //     return null;
        // }

    }
}
