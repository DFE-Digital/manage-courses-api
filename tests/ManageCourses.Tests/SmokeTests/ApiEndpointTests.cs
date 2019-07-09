using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Tests.TestUtilities;
using GovUk.Education.ManageCourses.UcasCourseImporter;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using GovUk.Education.ManageCourses.Domain.Models;

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
            const string ucasProviderCode = "ABC";
            var model = new UcasProviderEnrichmentPostModel();
            model.EnrichmentModel = new ProviderEnrichmentModel
            {
                TrainWithUs = "wqeqwe",
                TrainWithDisability = "werwer"
            };
            await apiClient.Enrichment_SaveProviderAsync(ucasProviderCode, model);
        }
        [Test]
        public async Task EnrichmentPublishTest()
        {
            SetupSmokeTestData();
            var apiClient = await BuildSigninAwareClient();
            const string ucasProviderCode = "ABC";
            var model = new UcasProviderEnrichmentPostModel();
            model.EnrichmentModel = new ProviderEnrichmentModel
            {
                TrainWithUs = "wqeqwe",
                TrainWithDisability = "werwer"
            };
            await apiClient.Enrichment_SaveProviderAsync(ucasProviderCode, model);

            var result = await apiClient.Publish_PublishCoursesToSearchAndCompareAsync(ucasProviderCode);
            result.Should().BeTrue();
        }
        [Test][Ignore("needs search and compare environment up and running for this test to pass")]
        public async Task CoursePublishTest()
        {
            SetupSmokeTestData();
            var apiClient = await BuildSigninAwareClient();
            const string ucasProviderCode = "ABC";
            const string ucasCourseCode = "CC101";
            var postModel = new CourseEnrichmentModel
            {
                AboutCourse = "'Begin at the beginning,' the King said, very gravely, 'and go on till you come to the end: then stop.'",
            };
            await apiClient.Enrichment_SaveCourseAsync(ucasProviderCode, ucasCourseCode, postModel);

//            var result = await apiClient(ucasInstitutionCode, ucasCourseCode);
//            result.Should().BeTrue();
        }
        [Test]
        public async Task GetSearchAndCompareCourseTest()
        {
            SetupSmokeTestData();
            var apiClient = await BuildSigninAwareClient();
            const string ucasProviderCode = "ABC";
            const string ucasCourseCode = "XYZ";
            var postModel = new CourseEnrichmentModel
            {
                AboutCourse = "'Begin at the beginning,' the King said, very gravely, 'and go on till you come to the end: then stop.'",
            };
            await apiClient.Enrichment_SaveCourseAsync(ucasProviderCode, ucasCourseCode, postModel);

            var result = await apiClient.Publish_GetSearchAndCompareCourseAsync(ucasProviderCode, ucasCourseCode);

            result.ProgrammeCode.Should().BeEquivalentTo(ucasCourseCode);
            result.Provider.ProviderCode.Should().BeEquivalentTo(ucasProviderCode);
        }
        [Test]
        public async Task EnrichmentLoadTest()
        {
            SetupSmokeTestData();
            var apiClient = await BuildSigninAwareClient();
            const string ucasProviderCode = "ABC";
            var model = new UcasProviderEnrichmentPostModel();
            model.EnrichmentModel = new ProviderEnrichmentModel
            {
                TrainWithUs = "wqeqwe",
                TrainWithDisability = "werwer"
            };
            await apiClient.Enrichment_SaveProviderAsync(ucasProviderCode, model);

            var loadedEnrichment = await apiClient.Enrichment_GetProviderAsync(ucasProviderCode);

            loadedEnrichment.Should().NotBeNull();
        }

        [Test]
        public async Task CourseEnrichmentRoundTrip()
        {
            SetupSmokeTestData();
            var apiClient = await BuildSigninAwareClient();
            const string ucasProviderCode = "ABC";
            const string ucasCourseCode = "CC101";
            var postModel = new CourseEnrichmentModel
            {
                AboutCourse = "'Begin at the beginning,' the King said, very gravely, 'and go on till you come to the end: then stop.'",
            };
            await apiClient.Enrichment_SaveCourseAsync(ucasProviderCode, ucasCourseCode, postModel);

            var loadedEnrichment = await apiClient.Enrichment_GetCourseAsync(ucasProviderCode, ucasCourseCode);

            loadedEnrichment.Should().NotBeNull();
            loadedEnrichment.EnrichmentModel.Should().NotBeNull();
            loadedEnrichment.EnrichmentModel.AboutCourse.Should().Be(postModel.AboutCourse);
        }


        [Test]
        public async Task Invite()
        {
            var accessToken = TestConfig.ApiKey;

            var client = BuildClient(accessToken);

            Context.AddTestReferenceData(Email, CurrentRecruitmentCycle);
            Context.Save();

            // does not throw... nb. Assert.DoesNotThrow does not support async voids
           await client.Invite_IndexAsync(Email);
        }

        [Test]
        public void Invite_badAccesCode_404()
        {
            var accessToken = "badAccesCode";

            var client = BuildClient(accessToken);

            Func<Task> act = async () => await client.Invite_IndexAsync(Email);

            var nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
            nameValueCollection["email"] = Email;

            var msg = $"API POST Failed uri {LocalWebHost.Address}/api/invite?{nameValueCollection.ToString()}";

            act.Should().Throw<ManageCoursesApiException>().WithMessage(msg).Which.StatusCode.Equals(HttpStatusCode.NotFound);
        }

        [Test]
        public void Invite_noAccesCode_401()
        {
            const string accessToken = "";

            var client = BuildClient(accessToken);

            Func<Task> act = async () => await client.Invite_IndexAsync(Email);

            var nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
            nameValueCollection["email"] = Email;

            var msg = $"API POST Failed uri {LocalWebHost.Address}/api/invite?{nameValueCollection.ToString()}";
            act.Should().Throw<ManageCoursesApiException>().WithMessage(msg).Which.StatusCode.Equals(HttpStatusCode.Unauthorized);
        }

        private void SetupSmokeTestData()
        {
            // don't use the retrying context because the migrator hasn't been updated to use the retry strategy
            var migratorContext = ContextLoader.GetDbContext(Config, false);

            // Not using CurrentRecruitmentCycle from base class because it doesn't share this context
            var currentRecruitmentCycle = migratorContext
                .RecruitmentCycles
                .Single(rc => rc.Year == RecruitmentCycle.CurrentYear);
            migratorContext.AddTestReferenceData(TestConfig.SignInUsername, currentRecruitmentCycle);
            migratorContext.Save();

            var ucasDataMigrator = new UcasDataMigrator(migratorContext,
                new Mock<Serilog.ILogger>().Object,
                TestPayloadBuilder.MakeSimpleUcasPayload(),
                null,
                currentRecruitmentCycle);

            ucasDataMigrator.UpdateUcasData();
        }

        private async Task<ManageCoursesApiClient> BuildSigninAwareClient()
        {
            var communicator = new DfeSignInCommunicator(TestConfig);
            var accessToken = await communicator.GetAccessToken(TestConfig);

            var client = BuildClient(accessToken);

            return client;
        }
    }
}
