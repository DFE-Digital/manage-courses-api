using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Tests.TestUtilities;
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
    public class ApiEndpointTests : ApiSmokeTestBase
    {
            const string Email = "feddie.krueger@example.org";

        [Test]
        public void DataExport_WithEmptyCampus()
        {
            var dfeSignInConfig = Config.GetSection("credentials").GetSection("dfesignin");

            var communicator = new DfeSignInCommunicator(dfeSignInConfig["host"], dfeSignInConfig["redirect_host"], dfeSignInConfig["clientid"], dfeSignInConfig["clientsecret"]);
            var accessToken = communicator.GetAccessTokenAsync(dfeSignInConfig["username"], dfeSignInConfig["password"]).Result;


            var httpClient = new HttpClient();
            var apiKeyAccessToken = Config["api:key"];


            var clientImport = new ManageCoursesApiClient(new MockApiClientConfiguration(apiKeyAccessToken), httpClient);
            clientImport.BaseUrl = LocalWebHost.Address;

            clientImport.Data_ImportReferenceDataAsync(TestPayloadBuilder.MakeReferenceDataPayload(dfeSignInConfig["username"])).Wait();
            clientImport.Data_ImportAsync(TestPayloadBuilder.MakeSimpleUcasPayload()).Wait();


            var clientExport = new ManageCoursesApiClient(new MockApiClientConfiguration(accessToken), httpClient);
            clientExport.BaseUrl = LocalWebHost.Address;

            var export = clientExport.Data_ExportByOrganisationAsync("ABC").Result;

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
            var accessToken = "badAccesCode";

            var httpClient = new HttpClient();

            var client = new ManageCoursesApiClient(new MockApiClientConfiguration(accessToken), httpClient);
            client.BaseUrl = LocalWebHost.Address;


            Assert.That(() => client.Data_ExportByOrganisationAsync("ABC"),
                Throws.TypeOf<SwaggerException>()
                    .With.Message.EqualTo("The HTTP status code of the response was not expected (404)."));

            Assert.That(() => client.Data_ImportAsync(new UcasPayload()),
                Throws.TypeOf<SwaggerException>()
                    .With.Message.EqualTo("The HTTP status code of the response was not expected (404)."));

        }

        [Test]
        public async Task Invite()
        {
            var accessToken = Config["api:key"];

            var httpClient = new HttpClient();

            var client = new ManageCoursesApiClient(new MockApiClientConfiguration(accessToken), httpClient);
            client.BaseUrl = LocalWebHost.Address;

            client.Data_ImportReferenceDataAsync(TestPayloadBuilder.MakeReferenceDataPayload(Email)).Wait();

            var result = await client.Invite_IndexAsync(Email);

            Assert.AreEqual(200, result.StatusCode);

            var client2 = new ManageCoursesApiClient(new MockApiClientConfiguration("accessToken"), httpClient);
            client2.BaseUrl = LocalWebHost.Address;
        }

        [Test]
        public void Invite_badAccesCode_404()
        {
            var accessToken = "badAccesCode";

            var httpClient = new HttpClient();

            var client = new ManageCoursesApiClient(new MockApiClientConfiguration(accessToken), httpClient);
            client.BaseUrl = LocalWebHost.Address;


            Assert.That(() => client.Invite_IndexAsync(Email),
                Throws.TypeOf<SwaggerException>()
                    .With.Message.EqualTo("The HTTP status code of the response was not expected (404)."));
        }

        [Test]
        public void Invite_noAccesCode_401()
        {
            var accessToken = "";

            var httpClient = new HttpClient();

            var client = new ManageCoursesApiClient(new MockApiClientConfiguration(accessToken), httpClient);
            client.BaseUrl = LocalWebHost.Address;

            Assert.That(() => client.Invite_IndexAsync(Email),
                Throws.TypeOf<SwaggerException>()
                    .With.Message.EqualTo("The HTTP status code of the response was not expected (401)."));
        }

        private class MockApiClientConfiguration : IManageCoursesApiClientConfiguration
        {
            private string accessToken;

            public MockApiClientConfiguration(string accessToken)
            {
                this.accessToken = accessToken;
            }

            public Task<string> GetAccessTokenAsync()
            {
                return Task.FromResult(accessToken);
            }
        }
    }
}