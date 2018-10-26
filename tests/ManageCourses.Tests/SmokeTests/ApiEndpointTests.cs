using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Tests.TestUtilities;
using GovUk.Education.ManageCourses.UcasCourseImporter;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.SmokeTests
{
    /// <summary>
    /// These tests poke the http endpoints of the api whilst the api is running in a
    /// a captive web host.
    /// </summary>
    [TestFixture]
    [Category("Smoke")]
    [Explicit]
    public partial class ApiEndpointTests : ApiSmokeTestBase
    {
        private const string Email = "feddie.krueger@example.org";

        [Test]
        public async Task EnrichmentSaveTest()
        {
            SetupSmokeTestData();
            var apiClient = await BuildSigninAwareClient();
            const string ucasInstitutionCode = "ABC";
            var model = new UcasInstitutionEnrichmentPostModel();
            model.EnrichmentModel = new InstitutionEnrichmentModel
            {
                TrainWithUs = "wqeqwe",
                TrainWithDisability = "werwer"
            };
            apiClient.Enrichment_SaveInstitutionAsync(ucasInstitutionCode, model);
        }
        [Test]
        public async Task EnrichmentPublishTest()
        {
            SetupSmokeTestData();
            var apiClient = await BuildSigninAwareClient();
            const string ucasInstitutionCode = "ABC";
            var model = new UcasInstitutionEnrichmentPostModel();
            model.EnrichmentModel = new InstitutionEnrichmentModel
            {
                TrainWithUs = "wqeqwe",
                TrainWithDisability = "werwer"
            };
            apiClient.Enrichment_SaveInstitutionAsync(ucasInstitutionCode, model);

            var result = await apiClient.Publish_PublishCoursesToSearchAndCompareAsync(ucasInstitutionCode);
            result.Should().BeTrue();
        }
        [Test][Ignore("needs search and compare environment up and running for this test to pass")]
        public async Task CoursePublishTest()
        {
            SetupSmokeTestData();
            var apiClient = await BuildSigninAwareClient();
            const string ucasInstitutionCode = "ABC";
            const string ucasCourseCode = "CC101";
            var postModel = new CourseEnrichmentModel
            {
                AboutCourse = "'Begin at the beginning,' the King said, very gravely, 'and go on till you come to the end: then stop.'",
            };
            apiClient.Enrichment_SaveCourseAsync(ucasInstitutionCode, ucasCourseCode, postModel);

//            var result = await apiClient(ucasInstitutionCode, ucasCourseCode);
//            result.Should().BeTrue();
        }
        [Test]
        public async Task GetSearchAndCompareCourseTest()
        {
            SetupSmokeTestData();
            var apiClient = await BuildSigninAwareClient();
            const string ucasInstitutionCode = "ABC";
            const string ucasCourseCode = "XYZ";
            var postModel = new CourseEnrichmentModel
            {
                AboutCourse = "'Begin at the beginning,' the King said, very gravely, 'and go on till you come to the end: then stop.'",
            };
            apiClient.Enrichment_SaveCourseAsync(ucasInstitutionCode, ucasCourseCode, postModel);

            var result = await apiClient.Publish_GetSearchAndCompareCourseAsync(ucasInstitutionCode, ucasCourseCode);

            result.ProgrammeCode.Should().BeEquivalentTo(ucasCourseCode);
            result.Provider.ProviderCode.Should().BeEquivalentTo(ucasInstitutionCode);
        }
        [Test]
        public async Task EnrichmentLoadTest()
        {
            SetupSmokeTestData();
            var apiClient = await BuildSigninAwareClient();
            const string ucasInstitutionCode = "ABC";
            var model = new UcasInstitutionEnrichmentPostModel();
            model.EnrichmentModel = new InstitutionEnrichmentModel
            {
                TrainWithUs = "wqeqwe",
                TrainWithDisability = "werwer"
            };
            apiClient.Enrichment_SaveInstitutionAsync(ucasInstitutionCode, model);

            var loadedEnrichment = await apiClient.Enrichment_GetInstitutionAsync(ucasInstitutionCode);

            loadedEnrichment.Should().NotBeNull();
        }

        [Test]
        public async Task CourseEnrichmentRoundTrip()
        {
            SetupSmokeTestData();
            var apiClient = await BuildSigninAwareClient();
            const string ucasInstitutionCode = "ABC";
            const string ucasCourseCode = "CC101";
            var postModel = new CourseEnrichmentModel
            {
                AboutCourse = "'Begin at the beginning,' the King said, very gravely, 'and go on till you come to the end: then stop.'",
            };
            apiClient.Enrichment_SaveCourseAsync(ucasInstitutionCode, ucasCourseCode, postModel);

            var loadedEnrichment = await apiClient.Enrichment_GetCourseAsync(ucasInstitutionCode, ucasCourseCode);

            loadedEnrichment.Should().NotBeNull();
            loadedEnrichment.EnrichmentModel.Should().NotBeNull();
            loadedEnrichment.EnrichmentModel.AboutCourse.Should().Be(postModel.AboutCourse);
        }


        [Test]
        public void Invite()
        {
            var accessToken = TestConfig.ApiKey;

            var httpClient = new HttpClient();

            var client = new ManageCoursesApiClient(new MockApiClientConfiguration(accessToken, LocalWebHost.Address), httpClient);

            Context.AddTestReferenceData(Email);
            Context.Save();

            // does not throw... nb. Assert.DoesNotThrow does not support async voids
            client.Invite_IndexAsync(Email);
        }

        [Test]
        public void Invite_badAccesCode_404()
        {
            var accessToken = "badAccesCode";

            var httpClient = new HttpClient();

            var client = new ManageCoursesApiClient(new MockApiClientConfiguration(accessToken, LocalWebHost.Address), httpClient);


            Assert.That(() => client.Invite_IndexAsync(Email).Result,
                Throws.TypeOf<ManageCoursesApiException>()
                    .With.Message.EqualTo("The HTTP status code of the response was not expected (404)."));
        }

        [Test]
        public void Invite_noAccesCode_401()
        {
            const string accessToken = "";

            var httpClient = new HttpClient();

            var client = new ManageCoursesApiClient(new MockApiClientConfiguration(accessToken, LocalWebHost.Address), httpClient);

            Assert.That(() => client.Invite_IndexAsync(Email).Result,
                Throws.TypeOf<ManageCoursesApiException>()
                    .With.Message.EqualTo("The HTTP status code of the response was not expected (401)."));
        }

        private void SetupSmokeTestData()
        {
            Context.AddTestReferenceData(TestConfig.SignInUsername);
            Context.Save();

            new UcasDataMigrator(Context, new Mock<Serilog.ILogger>().Object,TestPayloadBuilder.MakeSimpleUcasPayload()).UpdateUcasData();
        }

        private async Task<ManageCoursesApiClient> BuildSigninAwareClient()
        {
            var communicator = new DfeSignInCommunicator(TestConfig);
            var accessToken = await communicator.GetAccessTokenAsync(TestConfig);
            var client = new ManageCoursesApiClient(new MockApiClientConfiguration(accessToken, LocalWebHost.Address), new HttpClient());

            return client;
        }

        private ManageCoursesApiClient BuildApiKeyClient(string apiKey = null)
        {
            var apiKeyAccessToken = apiKey ?? TestConfig.ApiKey;
            var client = new ManageCoursesApiClient(new MockApiClientConfiguration(apiKeyAccessToken, LocalWebHost.Address), new HttpClient());

            return client;
        }
    }
}
