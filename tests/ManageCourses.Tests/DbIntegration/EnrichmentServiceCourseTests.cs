﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Services.Data;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.UcasCourseImporter;
using GovUk.Education.ManageCourses.Xls.Domain;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.DbIntegration
{
    [TestFixture]
    [Category("Integration")]
    [Category("Integration_DB")]
    public class EnrichmentServiceCourseTests : DbIntegrationTestBase
    {
        private Provider _ucasInstitution;
        private const string ProviderInstCode = "HNY1";
        private const string AccreditingInstCode = "TRILU";
        private const string UcasCourseCode = "TK101";
        private const string OtherCourseCode = "D0H";

        private const string Email = "12345@example.org";

        protected override void Setup()
        {
            var accreditingInstitution = new Provider
            {
                ProviderName = "Trilby University", // Universities can accredit courses provided by schools / SCITTs
                ProviderCode = AccreditingInstCode,
                RecruitmentCycle = CurrentRecruitmentCycle,
            };
            Context.Add(accreditingInstitution);

            const string providerInstCode = "HNY1";
            _ucasInstitution = new Provider
            {
                ProviderName = "Honey Lane School", // This is a school so has to have a university accredit the courses it offers
                ProviderCode = providerInstCode,
                Courses = new List<Course>
                {
                    new Course
                    {
                        CourseCode = UcasCourseCode,
                        Name = "Conscious control of telekenisis",
                        AccreditingProvider = accreditingInstitution,
                    },
                    new Course
                    {
                        CourseCode = OtherCourseCode,
                        Name = "Daemons and Gremlins",
                    },
                },
                RecruitmentCycle = CurrentRecruitmentCycle,
            };
            Context.Add(_ucasInstitution);

            var user = new User
            {
                Email = Email,
            };
            Context.Add(user);

            var org = new Organisation
            {
                Name = "Bucks Mega Org",
                OrgId = "BMO1",
                OrganisationUsers = new List<OrganisationUser>
                {
                    new OrganisationUser
                    {
                        User = user,
                    },
                },
                OrganisationProviders = new List<OrganisationProvider>
                {
                    new OrganisationProvider
                    {
                        Provider = _ucasInstitution,
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
            var enrichmentService = new EnrichmentService(Context);

            var courseEnrichment = enrichmentService.GetCourseEnrichment(ProviderInstCode, UcasCourseCode.ToLower(), Email);
            courseEnrichment.CourseCode.Should().BeNull("we haven't enriched this course yet");
            var emptyMetadata = enrichmentService.GetCourseEnrichmentMetadata(ProviderInstCode, Email);
            emptyMetadata.Count.Should().Be(0, "we haven't enriched any courses yet");

            //test saving sparse model
            var sourceModel = new CourseEnrichmentModel
            {
                AboutCourse = "About Course Text",
            };
            enrichmentService.SaveCourseEnrichment(sourceModel, ProviderInstCode.ToLower(), UcasCourseCode.ToLower(), Email);

            //test saving full model
            sourceModel.InterviewProcess = "eg InterviewProcess";
            sourceModel.HowSchoolPlacementsWork = "eg HowSchoolPlacementsWork";
            sourceModel.CourseLength = "eg CourseLength";
            sourceModel.FeeUkEu = 1.234m;
            sourceModel.FeeInternational = 42000.24m;
            sourceModel.SalaryDetails = "eg SalaryDetails";
            sourceModel.FeeDetails = "eg FeeDetails";
            sourceModel.FinancialSupport = "eg FinancialSupport";
            sourceModel.Qualifications = "eg Qualifications";
            sourceModel.PersonalQualities = "eg PersonalQualities";
            sourceModel.OtherRequirements = "eg OtherRequirements";
            enrichmentService.SaveCourseEnrichment(sourceModel, ProviderInstCode.ToLower(), UcasCourseCode.ToLower(), Email);

            //test get
            var courseEnrichmentMetadata = enrichmentService.GetCourseEnrichmentMetadata(ProviderInstCode, Email);
            courseEnrichmentMetadata.Count.Should().Be(1, "we have enriched one course");
            courseEnrichmentMetadata.Single().Status.Should().Be(EnumStatus.Draft);
            var result = enrichmentService.GetCourseEnrichment(ProviderInstCode.ToLower(), UcasCourseCode.ToLower(), Email);
            result.Should().NotBeNull();
            result.EnrichmentModel.Should().NotBeNull();
            result.EnrichmentModel.AboutCourse.Should().BeEquivalentTo(sourceModel.AboutCourse);
            result.EnrichmentModel.InterviewProcess.Should().BeEquivalentTo(sourceModel.InterviewProcess);
            result.EnrichmentModel.HowSchoolPlacementsWork.Should().BeEquivalentTo(sourceModel.HowSchoolPlacementsWork);
            result.EnrichmentModel.CourseLength.Should().BeEquivalentTo(sourceModel.CourseLength);
            result.EnrichmentModel.FeeUkEu.Should().Be(sourceModel.FeeUkEu);
            result.EnrichmentModel.FeeInternational.Should().Be(sourceModel.FeeInternational);
            result.EnrichmentModel.SalaryDetails.Should().BeEquivalentTo(sourceModel.SalaryDetails);
            result.EnrichmentModel.FeeDetails.Should().BeEquivalentTo(sourceModel.FeeDetails);
            result.EnrichmentModel.FinancialSupport.Should().BeEquivalentTo(sourceModel.FinancialSupport);
            result.EnrichmentModel.Qualifications.Should().BeEquivalentTo(sourceModel.Qualifications);
            result.EnrichmentModel.PersonalQualities.Should().BeEquivalentTo(sourceModel.PersonalQualities);
            result.EnrichmentModel.OtherRequirements.Should().BeEquivalentTo(sourceModel.OtherRequirements);
            result.LastPublishedTimestampUtc.Should().BeNull();
            result.Status.Should().BeEquivalentTo(EnumStatus.Draft);

            //test update
            sourceModel.AboutCourse = "About Course Text updated";

            enrichmentService.SaveCourseEnrichment(sourceModel, ProviderInstCode.ToLower(), UcasCourseCode, Email);
            var updateResult = enrichmentService.GetCourseEnrichment(ProviderInstCode, UcasCourseCode, Email);
            updateResult.EnrichmentModel.AboutCourse.Should().BeEquivalentTo(sourceModel.AboutCourse);
            updateResult.LastPublishedTimestampUtc.Should().BeNull();

            //publish
            var publishResults = enrichmentService.PublishCourseEnrichment(ProviderInstCode.ToLower(), UcasCourseCode, Email);
            publishResults.Should().BeTrue();
            var publishedMetadata = enrichmentService.GetCourseEnrichmentMetadata(ProviderInstCode, Email);
            publishedMetadata.Count.Should().Be(1, "we have enriched one course");
            publishedMetadata.Single().Status.Should().Be(EnumStatus.Published);
            var publishRecord = enrichmentService.GetCourseEnrichment(ProviderInstCode.ToLower(), UcasCourseCode, Email);
            publishRecord.Status.Should().BeEquivalentTo(EnumStatus.Published);
            publishRecord.LastPublishedTimestampUtc.Should().NotBeNull();
            var publishedRecord = Context.Courses.Single(c => c.CourseCode == UcasCourseCode && c.Provider.ProviderCode == ProviderInstCode);
            publishedRecord.ChangedAt.Should().BeCloseTo(publishRecord.UpdatedTimestampUtc.Value, 5000);

            //test save again after publish
            enrichmentService.SaveCourseEnrichment(sourceModel, ProviderInstCode.ToLower(), UcasCourseCode, Email);
            var updateResult2 = enrichmentService.GetCourseEnrichment(ProviderInstCode, UcasCourseCode, Email);
            updateResult2.EnrichmentModel.AboutCourse.Should().BeEquivalentTo(sourceModel.AboutCourse);
            updateResult2.EnrichmentModel.OtherRequirements.Should().BeEquivalentTo(sourceModel.OtherRequirements);
            updateResult2.Status.Should().BeEquivalentTo(EnumStatus.Draft);
            updateResult2.LastPublishedTimestampUtc.ToString().Should().BeEquivalentTo(publishRecord.LastPublishedTimestampUtc.ToString());

            //check number of records generated from this
            var draftCount = Context.CourseEnrichments.Count(x => x.Status == EnumStatus.Draft);
            var publishedCount = Context.CourseEnrichments.Count(x => x.Status == EnumStatus.Published);
            publishedCount.Should().Be(1);
            draftCount.Should().Be(1);

            // test saving & loading a different course in the same institution
            var nextCourseModel = new CourseEnrichmentModel
            {
                AboutCourse = "Some other course",
            };
            enrichmentService.SaveCourseEnrichment(nextCourseModel, ProviderInstCode.ToLower(), OtherCourseCode, Email);
            var twoCourseMetadata = enrichmentService.GetCourseEnrichmentMetadata(ProviderInstCode, Email);
            twoCourseMetadata.Count.Should().Be(2, "we have enriched two courses in this institution");
            var nextCourseGet = enrichmentService.GetCourseEnrichment(ProviderInstCode.ToLower(), OtherCourseCode, Email);
            nextCourseGet.Should().NotBeNull();
            nextCourseGet.EnrichmentModel.Should().NotBeNull();
            nextCourseGet.EnrichmentModel.AboutCourse.Should().BeEquivalentTo(nextCourseModel.AboutCourse);
        }

        [Test]
        [TestCase("", "")]
        [TestCase(null, null)]
        [TestCase("eqweqw", "qweqweq")]
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

            Assert.Throws<ArgumentException>(() => enrichmentService.GetCourseEnrichment(instCode, UcasCourseCode, email));
        }

        [Test]
        [TestCase("", "")]
        [TestCase(null, null)]
        [TestCase("eqweqw", "qweqweq")]
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
            var dataService = new DataService(Context, enrichmentService, new Mock<ILogger<DataService>>().Object);
            var sourceModel = new CourseEnrichmentModel
            {
                AboutCourse = aboutCourseText,
            };
            enrichmentService.SaveCourseEnrichment(sourceModel, _ucasInstitution.ProviderCode, UcasCourseCode, Email);

            // Act
            var ucasPayload = new UcasPayload
            {
                // todo: test with change of this institution: https://trello.com/c/e1FwXuYk/133-ucas-institutions-dont-get-updated-during-ucas-import
                Institutions = new List<UcasInstitution>
                {
                    new UcasInstitution
                    {
                        InstCode = _ucasInstitution.ProviderCode,
                        InstFull = "Rebranded Provider",
                    },
                    new UcasInstitution
                    {
                        InstCode = AccreditingInstCode,
                        InstFull = "Rebranded Accrediting Provider",
                    },
                },
                Courses = new List<UcasCourse>
                {
                    new UcasCourse
                    {
                        InstCode = _ucasInstitution.ProviderCode,
                        CrseCode = "CC11",
                        AccreditingProvider = AccreditingInstCode,
                    },
                },
            };
            new UcasDataMigrator(Context, new Mock<Serilog.ILogger>().Object, ucasPayload).UpdateUcasData();

            // Assert
            var res = enrichmentService.GetCourseEnrichment(_ucasInstitution.ProviderCode, UcasCourseCode, Email);
            res.EnrichmentModel.AboutCourse.Should().Be(sourceModel.AboutCourse);
        }
    }
}
