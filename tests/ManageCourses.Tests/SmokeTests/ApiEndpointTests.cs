using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Tests.TestUtilities;
using Microsoft.Extensions.Configuration;
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
        public void DataExport_WithEmptyCampus()
        {
            SetupSmokeTestData();

            var apiClient = BuildSigninAwareClient();

            var export = apiClient.Data_ExportByOrganisationAsync("ABC").Result;

            Assert.AreEqual("123", export.OrganisationId, "OrganisationId should be retrieved");
            Assert.AreEqual("Joe's school @ UCAS", export.OrganisationName, "OrganisationName should be retrieved");
            Assert.AreEqual("ABC", export.UcasCode, "UcasCode should be retrieved");

            Assert.AreEqual(1, export.ProviderCourses.Count, "Expecting exactly one in ProviderCourses");
            var course = export.ProviderCourses.Single();
            Assert.AreEqual(1, course.CourseDetails.Count, "Expecting exactly one in ProviderCourses.CourseDetails");
            var details = course.CourseDetails.Single();

            Assert.AreEqual("Joe's course for Primary teachers", details.CourseTitle, "ProviderCourses.CourseDetails.CourseTitle should be retrieved");

            Assert.AreEqual(1, details.Variants.Count, "Expecting exactly one in ProviderCourses.CourseDetails.Variants");
            var variant = details.Variants.Single();

            Assert.AreEqual("XYZ", variant.UcasCode, "ProviderCourses.CourseDetails.Variants.UcasCode should be retrieved");

            Assert.AreEqual(1, variant.Campuses.Count, "Expecting exactly one in ProviderCourses.CourseDetails.Variants.Campuses");
            var campus = variant.Campuses.Single();

            Assert.AreEqual("", campus.Code, "ProviderCourses.CourseDetails.Variants.Campuses.Code should be retrieved");
            Assert.AreEqual("Main campus site", campus.Name, "ProviderCourses.CourseDetails.Variants.Campuses.Name should be retrieved");
        }

        [Test]
        public void DataExport_badAccesCode_404()
        {
            var apiClient = BuildApiKeyClient("badAccesCode");

            Assert.That(() => apiClient.Data_ExportByOrganisationAsync("ABC"),
                Throws.TypeOf<SwaggerException>()
                    .With.Message.EqualTo("The HTTP status code of the response was not expected (404)."));

            Assert.That(() => apiClient.Data_ImportAsync(new UcasPayload()),
                Throws.TypeOf<SwaggerException>()
                    .With.Message.EqualTo("The HTTP status code of the response was not expected (404)."));
        }

        [Test]
        public async Task EnrichmentSaveTest()
        {
            SetupSmokeTestData();
            var apiClient = BuildSigninAwareClient();
            const string ucasInstitutionCode = "ABC";
            var model = new UcasInstitutionEnrichmentPostModel();
            model.EnrichmentModel = new InstitutionEnrichmentModel
            {
                TrainWithUs = "wqeqwe",
                TrainWithDisability = "werwer"
            };
            await apiClient.Enrichment_SaveInstitutionAsync(ucasInstitutionCode, model);
        }
        [Test]
        public async Task EnrichmentPublishTest()
        {
            SetupSmokeTestData();
            var apiClient = BuildSigninAwareClient();
            const string ucasInstitutionCode = "ABC";
            var model = new UcasInstitutionEnrichmentPostModel();
            model.EnrichmentModel = new InstitutionEnrichmentModel
            {
                TrainWithUs = "wqeqwe",
                TrainWithDisability = "werwer"
            };
            await apiClient.Enrichment_SaveInstitutionAsync(ucasInstitutionCode, model);

            var result = await apiClient.Enrichment_PublishAsync(ucasInstitutionCode);
            result.Should().NotBeNull();
        }

        [Test]
        public async Task EnrichmentLoadTest()
        {
            SetupSmokeTestData();
            var apiClient = BuildSigninAwareClient();
            const string ucasInstitutionCode = "ABC";
            var loadedEnrichment = await apiClient.Enrichment_GetInstitutionAsync(ucasInstitutionCode);
            loadedEnrichment.Should().NotBeNull();
        }

        [Test]
        public async Task Invite()
        {
            var accessToken = Config["api:key"];

            var httpClient = new HttpClient();

            var client = new ManageCoursesApiClient(new MockApiClientConfiguration(accessToken), httpClient)
            {
                BaseUrl = LocalWebHost.Address
            };

            client.Data_ImportReferenceDataAsync(TestPayloadBuilder.MakeReferenceDataPayload(Email)).Wait();

            var result = await client.Invite_IndexAsync(Email);

            result.StatusCode.Should().Be(200);
        }

        [Test]
        public void Invite_badAccesCode_404()
        {
            var accessToken = "badAccesCode";

            var httpClient = new HttpClient();

            var client = new ManageCoursesApiClient(new MockApiClientConfiguration(accessToken), httpClient)
            {
                BaseUrl = LocalWebHost.Address
            };


            Assert.That(() => client.Invite_IndexAsync(Email),
                Throws.TypeOf<SwaggerException>()
                    .With.Message.EqualTo("The HTTP status code of the response was not expected (404)."));
        }

        [Test]
        public void Invite_noAccesCode_401()
        {
            const string accessToken = "";

            var httpClient = new HttpClient();

            var client = new ManageCoursesApiClient(new MockApiClientConfiguration(accessToken), httpClient)
            {
                BaseUrl = LocalWebHost.Address
            };

            Assert.That(() => client.Invite_IndexAsync(Email),
                Throws.TypeOf<SwaggerException>()
                    .With.Message.EqualTo("The HTTP status code of the response was not expected (401)."));
        }

        private void SetupSmokeTestData()
        {
            var dfeSignInConfig = GetSigninConfig(Config);
            var clientImport = BuildApiKeyClient();
            clientImport.Data_ImportReferenceDataAsync(TestPayloadBuilder.MakeReferenceDataPayload(dfeSignInConfig["username"])).Wait();
            clientImport.Data_ImportAsync(TestPayloadBuilder.MakeSimpleUcasPayload()).Wait();
        }

        private ManageCoursesApiClient BuildSigninAwareClient()
        {
            var dfeSignInConfig = GetSigninConfig(Config);
            var communicator = new DfeSignInCommunicator(dfeSignInConfig["host"], dfeSignInConfig["redirect_host"],
                dfeSignInConfig["clientid"], dfeSignInConfig["clientsecret"]);
            var accessToken = communicator.GetAccessTokenAsync(dfeSignInConfig["username"], dfeSignInConfig["password"]).Result;
            var clientExport = new ManageCoursesApiClient(new MockApiClientConfiguration(accessToken), new HttpClient())
            {
                BaseUrl = LocalWebHost.Address
            };
            return clientExport;
        }

        private ManageCoursesApiClient BuildApiKeyClient(string apiKey = null)
        {
            var apiKeyAccessToken = apiKey ?? Config["api:key"];
            var importClient = new ManageCoursesApiClient(new MockApiClientConfiguration(apiKeyAccessToken), new HttpClient())
            {
                BaseUrl = LocalWebHost.Address
            };
            return importClient;
        }

        private static IConfigurationSection GetSigninConfig(IConfiguration configuration)
        {
            return configuration.GetSection("credentials").GetSection("dfesignin");
        }
    }
}
