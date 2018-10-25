using System.Net.Http.Headers;
using System.Net.Http;
using GovUk.Education.ManageCourses.Api.Model;
// using GovUk.Education.ManageCourses.Domain.Models;
namespace GovUk.Education.ManageCourses.ApiClient
{
    public class ManageCoursesApiClient
    {
        private readonly IManageCoursesApiClientConfiguration _apiClientConfiguration;
        private HttpClient _httpClient;
        private string _baseUrl = "";

        public string BaseUrl
        {
            get { return _baseUrl; }
            set { _baseUrl = value; }
        }

        public ManageCoursesApiClient(IManageCoursesApiClientConfiguration apiClientConfiguration, HttpClient httpClient)
        {
            _apiClientConfiguration = apiClientConfiguration;
            _httpClient = httpClient;
            BaseUrl = _apiClientConfiguration.GetBaseUrl();
        }
        void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url)
        {
            var accessToken = _apiClientConfiguration.GetAccessTokenAsync().Result;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

    public System.Threading.Tasks.Task AcceptTerms_IndexAsync()
    {
        return System.Threading.Tasks.Task.CompletedTask;
    }
    public System.Threading.Tasks.Task AccessRequest_IndexAsync(AccessRequest request)
    {
        return System.Threading.Tasks.Task.CompletedTask;
    }
    public System.Threading.Tasks.Task Admin_ActionAccessRequestAsync(int accessRequestId)
    {
        return System.Threading.Tasks.Task.CompletedTask;
    }
    public System.Threading.Tasks.Task Admin_ActionManualActionRequestAsync(string requesterEmail, string targetEmail, string firstName, string lastName)
    {
        return System.Threading.Tasks.Task.CompletedTask;
    }
    public System.Threading.Tasks.Task<Domain.Models.Course> Courses_GetAsync(string instCode, string ucasCode)
    {
        return null;
    }
    public System.Threading.Tasks.Task<InstitutionCourses> Courses_GetAllAsync(string instCode)
    {
        return null;
    }

    public System.Threading.Tasks.Task<UcasInstitutionEnrichmentGetModel> Enrichment_GetInstitutionAsync(string ucasInstitutionCode)
    {
        return null;
    }
    public System.Threading.Tasks.Task Enrichment_SaveInstitutionAsync(string ucasInstitutionCode, UcasInstitutionEnrichmentPostModel model)
    {
        return System.Threading.Tasks.Task.CompletedTask;
    }
    public System.Threading.Tasks.Task<UcasCourseEnrichmentGetModel> Enrichment_GetCourseAsync(string ucasInstitutionCode, string ucasCourseCode)
    {
        return null;
    }
    public System.Threading.Tasks.Task Enrichment_SaveCourseAsync(string ucasInstitutionCode, string ucasCourseCode, CourseEnrichmentModel model)
    {
        return System.Threading.Tasks.Task.CompletedTask;
    }

    public System.Threading.Tasks.Task Invite_IndexAsync(string email)
    {
        return System.Threading.Tasks.Task.CompletedTask;
    }
    public System.Threading.Tasks.Task<UserOrganisation> Organisations_GetAsync(string instCode)
    {
        return null;
    }
    public System.Threading.Tasks.Task<Domain.Models.Institution> Organisations_GetUcasInstitutionAsync(string instCode)
    {
        return null;
    }
    public System.Threading.Tasks.Task<System.Collections.ObjectModel.ObservableCollection<UserOrganisation>> Organisations_GetAllAsync()
    {
        return null;
    }
    public System.Threading.Tasks.Task<bool> Publish_PublishCourseToSearchAndCompareAsync(string instCode, string courseCode)
    {
        return null;
    }
    public System.Threading.Tasks.Task<bool> Publish_PublishCoursesToSearchAndCompareAsync(string instCode)
    {
        return null;
    }
    public System.Threading.Tasks.Task<SearchAndCompare.Domain.Models.Course> Publish_GetSearchAndCompareCourseAsync(string instCode, string courseCode)
    {
        return null;
    }


    // async boilerplating

        public async System.Threading.Tasks.Task AcceptTerms_IndexAsync(System.Threading.CancellationToken cancellationToken) {

            return;
        }
        public async System.Threading.Tasks.Task AccessRequest_IndexAsync(AccessRequest request, System.Threading.CancellationToken cancellationToken) {

            return;
        }
        public async System.Threading.Tasks.Task Admin_ActionAccessRequestAsync(int accessRequestId, System.Threading.CancellationToken cancellationToken) {

            return;
        }
        public async System.Threading.Tasks.Task Admin_ActionManualActionRequestAsync(string requesterEmail, string targetEmail, string firstName, string lastName, System.Threading.CancellationToken cancellationToken) {

            return;
        }
        public async System.Threading.Tasks.Task<Domain.Models.Course> Courses_GetAsync(string instCode, string ucasCode, System.Threading.CancellationToken cancellationToken) {

            return null;
        }
        public async System.Threading.Tasks.Task<InstitutionCourses> Courses_GetAllAsync(string instCode, System.Threading.CancellationToken cancellationToken) {

            return null;
        }

        public async System.Threading.Tasks.Task<UcasInstitutionEnrichmentGetModel> Enrichment_GetInstitutionAsync(string ucasInstitutionCode, System.Threading.CancellationToken cancellationToken) {

            return null;
        }
        public async System.Threading.Tasks.Task Enrichment_SaveInstitutionAsync(string ucasInstitutionCode, UcasInstitutionEnrichmentPostModel model, System.Threading.CancellationToken cancellationToken) {

            return;
        }
        public async System.Threading.Tasks.Task<UcasCourseEnrichmentGetModel> Enrichment_GetCourseAsync(string ucasInstitutionCode, string ucasCourseCode, System.Threading.CancellationToken cancellationToken) {

            return null;
        }
        public async System.Threading.Tasks.Task Enrichment_SaveCourseAsync(string ucasInstitutionCode, string ucasCourseCode, CourseEnrichmentModel model, System.Threading.CancellationToken cancellationToken) {

            return;
        }

        public async System.Threading.Tasks.Task Invite_IndexAsync(string email, System.Threading.CancellationToken cancellationToken) {

            return;
        }
        public async System.Threading.Tasks.Task<UserOrganisation> Organisations_GetAsync(string instCode, System.Threading.CancellationToken cancellationToken) {

            return null;
        }
        public async System.Threading.Tasks.Task<Domain.Models.Institution> Organisations_GetUcasInstitutionAsync(string instCode, System.Threading.CancellationToken cancellationToken) {

            return null;
        }
        public async System.Threading.Tasks.Task<System.Collections.ObjectModel.ObservableCollection<UserOrganisation>> Organisations_GetAllAsync(System.Threading.CancellationToken cancellationToken) {

            return null;
        }
        public async System.Threading.Tasks.Task<bool> Publish_PublishCourseToSearchAndCompareAsync(string instCode, string courseCode, System.Threading.CancellationToken cancellationToken) {

            return false;
        }
        public async System.Threading.Tasks.Task<bool> Publish_PublishCoursesToSearchAndCompareAsync(string instCode, System.Threading.CancellationToken cancellationToken) {

            return false;
        }
        public async System.Threading.Tasks.Task<Domain.Models.Course> Publish_GetSearchAndCompareCourseAsync(string instCode, string courseCode, System.Threading.CancellationToken cancellationToken) {

            return null;
        }

    }
}
