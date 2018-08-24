using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Services.Data;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.DbIntegration
{
    [TestFixture]
    [Category("Integration")]
    [Category("Integration_DB")]
    [Explicit]
    public class EnrichmentServiceCourseTests : DbIntegrationTestBase
    {
        private UcasInstitution _ucasInstitution;
        private const string ProviderInstCode = "HNY1";
        private const string AccreditingInstCode = "TRILU";
        private const string UcasCourseCode = "451F";

        private const string Email = "12345@example.org";

        protected override void Setup()
        {
            var accreditingInstitution = new UcasInstitution
            {
                InstName = "Trilby University", // Universities can accredit courses provided by schools / SCITTs
                InstCode = AccreditingInstCode,
            };
            Context.Add(accreditingInstitution);

            const string providerInstCode = "HNY1";
            const string crseCode = "TK101";
            _ucasInstitution = new UcasInstitution
            {
                InstName = "Honey Lane School", // This is a school so has to have a university accredit the courses it offers
                InstCode = providerInstCode,
                UcasCourses = new List<UcasCourse>
                {
                    new UcasCourse
                    {
                        InstCode = providerInstCode,
                        CrseCode = crseCode,
                        CrseTitle = "Conscious control of telekenisis",
                        CourseCode = new CourseCode
                        {
                            InstCode = providerInstCode,
                            CrseCode = crseCode,
                        },
                        AccreditingProvider = AccreditingInstCode,
                    }
                }
            };
            Context.Add(_ucasInstitution);

            var user = new McUser
            {
                Email = Email,
            };
            Context.Add(user);

            var org = new McOrganisation
            {
                Name = "Bucks Mega Org",
                OrgId = "BMO1",
                McOrganisationUsers = new List<McOrganisationUser>
                {
                    new McOrganisationUser
                    {
                        McUser = user,
                    },
                },
                McOrganisationInstitutions = new List<McOrganisationInstitution>
                {
                    new McOrganisationInstitution
                    {
                        UcasInstitution = _ucasInstitution,
                    },
                }
            };
            Context.Add(org);

            Context.SaveChanges();
        }
        /// <summary>
        /// This is a happy path test for all enrichment functionality.
        /// This test ensures that the status is always correct at the right point in the workflow
        /// </summary>
        [Test]
        public void Test_CourseEnrichment_And_Publishing_Workflow()
        {
            const string aboutCourseText = "About Course Text";
            const string aboutCourseUpdatedText = "About Course Text updated";

            var enrichmentService = new EnrichmentService(Context);
            var model = new CourseEnrichmentModel
            {
                AboutCourse =aboutCourseText,
            };
            //test save
            enrichmentService.SaveCourseEnrichment(model, ProviderInstCode.ToLower(), UcasCourseCode.ToLower(), Email);

            //test get
            var result = enrichmentService.GetCourseEnrichment(ProviderInstCode.ToLower(), UcasCourseCode.ToLower(), Email);

            result.Should().NotBeNull();
            result.EnrichmentModel.Should().NotBeNull();
            result.EnrichmentModel.AboutCourse.Should().BeEquivalentTo(aboutCourseText);
            result.LastPublishedTimestampUtc.Should().BeNull();
            result.Status.Should().BeEquivalentTo(EnumStatus.Draft);

            //test update
            var updatedmodel = new CourseEnrichmentModel
            {
                AboutCourse = aboutCourseUpdatedText,
            };

            enrichmentService.SaveCourseEnrichment(updatedmodel, ProviderInstCode.ToLower(), UcasCourseCode, Email);
            var updateResult = enrichmentService.GetCourseEnrichment(ProviderInstCode, UcasCourseCode, Email);
            updateResult.EnrichmentModel.AboutCourse.Should().BeEquivalentTo(aboutCourseUpdatedText);
            updateResult.LastPublishedTimestampUtc.Should().BeNull();
            //publish
            var publishResults = enrichmentService.PublishCourseEnrichment(ProviderInstCode.ToLower(), UcasCourseCode, Email);
            publishResults.Should().BeTrue();
            var publishRecord = enrichmentService.GetCourseEnrichment(ProviderInstCode.ToLower(), UcasCourseCode, Email);
            publishRecord.Status.Should().BeEquivalentTo(EnumStatus.Published);
            publishRecord.LastPublishedTimestampUtc.Should().NotBeNull();
            //test save again after publish
            enrichmentService.SaveCourseEnrichment(model, ProviderInstCode.ToLower(), UcasCourseCode, Email);
            var updateResult2 = enrichmentService.GetCourseEnrichment(ProviderInstCode, UcasCourseCode, Email);
            updateResult2.EnrichmentModel.AboutCourse.Should().BeEquivalentTo(aboutCourseText);
            updateResult2.Status.Should().BeEquivalentTo(EnumStatus.Draft);
            updateResult2.LastPublishedTimestampUtc.ToString().Should().BeEquivalentTo(publishRecord.LastPublishedTimestampUtc.ToString());
            //check number of records generated from this
            var draftCount = Context.CourseEnrichments.Count(x => x.Status == EnumStatus.Draft);
            var publishedCount = Context.CourseEnrichments.Count(x => x.Status == EnumStatus.Published);
            publishedCount.Should().Be(1);
            draftCount.Should().Be(1);
        }
        [Test]
        [TestCase("eqweqw", "qweqweq")]
        public void Test_SaveCourseEnrichment_should_return_invalid_operation_exception(string instCode, string email)
        {
            const string aboutCourseText = "About Course Text";

            var enrichmentService = new EnrichmentService(Context);
            var model = new CourseEnrichmentModel
            {
                AboutCourse = aboutCourseText,
            };
            Assert.Throws<InvalidOperationException>(() => enrichmentService.SaveCourseEnrichment(model, instCode, UcasCourseCode, email));
        }
        [Test]
        [TestCase("", "")]
        [TestCase(null, null)]
        public void Test_SaveCourseEnrichment_should__argument_exception(string instCode, string email)
        {
            const string aboutCourseText = "About Course Text";

            var enrichmentService = new EnrichmentService(Context);
            var model = new CourseEnrichmentModel
            {
                AboutCourse = aboutCourseText,
            };

            Assert.Throws<ArgumentException>(() => enrichmentService.SaveCourseEnrichment(model, instCode, UcasCourseCode, email));
        }
        [Test]
        [TestCase("eqweqw", "qweqweq")]
        public void Test_GetCourseEnrichment_should_error(string instCode, string email)
        {
            var enrichmentService = new EnrichmentService(Context);

            Assert.Throws<InvalidOperationException>(() => enrichmentService.GetCourseEnrichment(instCode, UcasCourseCode, email));
        }
        [Test]
        [TestCase("eqweqw", "qweqweq")]
        public void Test_PublishCourseEnrichment_should_return_invalid_operation_exception(string instCode, string email)
        {
            var enrichmentService = new EnrichmentService(Context);

            Assert.Throws<InvalidOperationException>(() => enrichmentService.PublishCourseEnrichment(instCode, UcasCourseCode, email));
        }
        [Test]
        [TestCase("", "")]
        [TestCase(null, null)]
        public void Test_PublishCourseEnrichment_should_argument_exception(string instCode, string email)
        {
            var enrichmentService = new EnrichmentService(Context);
            Assert.Throws<ArgumentException>(() => enrichmentService.PublishCourseEnrichment(instCode, UcasCourseCode, email));
        }

        [Test]
        public void EnrichmentDataSurvivesDeleteAndRecreate()
        {
            const string aboutCourseText = "About Course Text";
            // Arrange
            var enrichmentService = new EnrichmentService(Context);
            var dataService = new DataService(Context, new UserDataHelper(), new Mock<ILogger<DataService>>().Object);
            var sourceModel = new CourseEnrichmentModel
            {
                AboutCourse = aboutCourseText,
            };
            enrichmentService.SaveCourseEnrichment(sourceModel, _ucasInstitution.InstCode, UcasCourseCode, Email);

            // Act
            var ucasPayload = new UcasPayload
            {
                // todo: test with change of this institution: https://trello.com/c/e1FwXuYk/133-ucas-institutions-dont-get-updated-during-ucas-import
                Institutions = new List<UcasInstitution>
                {
                    new UcasInstitution
                    {
                        InstCode = _ucasInstitution.InstCode,
                        InstName = "Rebranded Institution",
                    },
                    new UcasInstitution
                    {
                        InstCode = AccreditingInstCode,
                        InstName = "Rebranded Accrediting Institution",
                    },
                },
                Courses = new List<UcasCourse>
                {
                    new UcasCourse
                    {
                        InstCode = _ucasInstitution.InstCode,
                        CrseCode = "CC11",
                        AccreditingProvider = AccreditingInstCode,
                    },
                },
            };
            dataService.ProcessUcasPayload(ucasPayload);

            // Assert
            var res = enrichmentService.GetCourseEnrichment(_ucasInstitution.InstCode, UcasCourseCode, Email);
            res.EnrichmentModel.AboutCourse.Should().Be(sourceModel.AboutCourse);
        }
    }
}
