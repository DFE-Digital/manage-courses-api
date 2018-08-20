﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Domain.Models;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.DbIntegration
{
    [TestFixture]
    [Category("Integration")]
    [Category("Integration_DB")]
    [Explicit]
    public class EnrichmentServiceTests : DbIntegrationTestBase
    {
        private UcasInstitution _ucasInstitution;
        private const string ProviderInstCode = "HNY1";
        private const string AccreditingInstCode = "TRILU";

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
        public void Test_InstitutionEnrichment_workflow_should_not_error()
        {
            const string trainWithDisabilityText = "TrainWithDisabilily Text";
            const string trainWithUsText = "TrainWithUs Text";
            const string trainWithDisabilityUpdatedText = "TrainWithDisabilily Text updated";
            const string trainWithUsUpdatedText = "TrainWithUs Text updated";
            const string instDesc = "school1 description enrichement";

            var enrichmentService = new EnrichmentService(Context);
            var model = new UcasInstitutionEnrichmentPostModel
            {
                EnrichmentModel = new InstitutionEnrichmentModel
                {
                    TrainWithDisability = trainWithDisabilityText,
                    TrainWithUs = trainWithUsText,
                    AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>
                    {
                        new AccreditingProviderEnrichment
                        {
                            UcasInstitutionCode = AccreditingInstCode,
                            Description = instDesc,
                        }
                    }
                }
            };
            //test save
            enrichmentService.SaveInstitutionEnrichment(model, ProviderInstCode.ToLower(), Email);

            //test get
            var result = enrichmentService.GetInstitutionEnrichment(ProviderInstCode.ToLower(), Email);

            result.Should().NotBeNull();
            result.EnrichmentModel.Should().NotBeNull();
            result.EnrichmentModel.TrainWithDisability.Should().BeEquivalentTo(trainWithDisabilityText);
            result.EnrichmentModel.TrainWithUs.Should().BeEquivalentTo(trainWithUsText);
            result.LastPublishedTimestampUtc.Should().BeNull();
            result.Status.Should().BeEquivalentTo(EnumStatus.Draft);

            //test update
            var updatedmodel = new UcasInstitutionEnrichmentPostModel
            {
                EnrichmentModel = new InstitutionEnrichmentModel
                {
                    TrainWithDisability = trainWithDisabilityUpdatedText,
                    TrainWithUs = trainWithUsUpdatedText,
                    AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>
                    {
                        new AccreditingProviderEnrichment
                        {
                            UcasInstitutionCode = AccreditingInstCode,
                            Description = instDesc,
                        }
                    }
                }
            };

            enrichmentService.SaveInstitutionEnrichment(updatedmodel, ProviderInstCode.ToLower(), Email);
            var updateResult = enrichmentService.GetInstitutionEnrichment(ProviderInstCode, Email);
            updateResult.EnrichmentModel.TrainWithDisability.Should().BeEquivalentTo(trainWithDisabilityUpdatedText);
            updateResult.EnrichmentModel.TrainWithUs.Should().BeEquivalentTo(trainWithUsUpdatedText);
            updateResult.LastPublishedTimestampUtc.Should().BeNull();
            //publish
            var publishResults = enrichmentService.PublishInstitutionEnrichment(ProviderInstCode.ToLower(), Email);
            publishResults.Should().BeTrue();
            var publishRecord = enrichmentService.GetInstitutionEnrichment(ProviderInstCode.ToLower(), Email);
            publishRecord.Status.Should().BeEquivalentTo(EnumStatus.Published);
            publishRecord.LastPublishedTimestampUtc.Should().NotBeNull();
            //test save again after publish
            enrichmentService.SaveInstitutionEnrichment(model, ProviderInstCode.ToLower(), Email);
            var updateResult2 = enrichmentService.GetInstitutionEnrichment(ProviderInstCode, Email);
            updateResult2.EnrichmentModel.TrainWithDisability.Should().BeEquivalentTo(trainWithDisabilityText);
            updateResult2.EnrichmentModel.TrainWithUs.Should().BeEquivalentTo(trainWithUsText);
            updateResult2.Status.Should().BeEquivalentTo(EnumStatus.Draft);
            updateResult2.LastPublishedTimestampUtc.ToString().Should().BeEquivalentTo(publishRecord.LastPublishedTimestampUtc.ToString());
            //check number of records generated from this
            var draftCount = Context.InstitutionEnrichments.Count(x => x.Status == EnumStatus.Draft);
            var publishedCount = Context.InstitutionEnrichments.Count(x => x.Status == EnumStatus.Published);
            publishedCount.Should().Be(1);
            draftCount.Should().Be(1);
        }
        [Test]
        [TestCase("eqweqw", "qweqweq")]
        public void Test_SaveInstitutionEnrichment_should_return_invalid_operation_exception(string instCode, string email)
        {
            const string trainWithDisabilityText = "TrainWithDisabilily Text";
            const string trainWithUsText = "TrainWithUs Text";
            const string instDesc = "school1 description enrichement";

            var enrichmentService = new EnrichmentService(Context);
            var model = new UcasInstitutionEnrichmentPostModel
            {
                EnrichmentModel = new InstitutionEnrichmentModel
                {
                    TrainWithDisability = trainWithDisabilityText,
                    TrainWithUs = trainWithUsText,
                    AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>
                    {
                        new AccreditingProviderEnrichment
                        {
                            UcasInstitutionCode = AccreditingInstCode,
                            Description = instDesc,
                        }
                    }
                }
            };

            Assert.Throws<InvalidOperationException>(() => enrichmentService.SaveInstitutionEnrichment(model, instCode, email));
        }
        [Test]
        [TestCase("", "")]
        [TestCase(null, null)]
        public void Test_SaveInstitutionEnrichment_should__argument_exception(string instCode, string email)
        {
            const string trainWithDisabilityText = "TrainWithDisabilily Text";
            const string trainWithUsText = "TrainWithUs Text";
            const string instDesc = "school1 description enrichement";

            var enrichmentService = new EnrichmentService(Context);
            var model = new UcasInstitutionEnrichmentPostModel
            {
                EnrichmentModel = new InstitutionEnrichmentModel
                {
                    TrainWithDisability = trainWithDisabilityText,
                    TrainWithUs = trainWithUsText,
                    AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>
                    {
                        new AccreditingProviderEnrichment
                        {
                            UcasInstitutionCode = AccreditingInstCode,
                            Description = instDesc,
                        }
                    }
                }
            };

            Assert.Throws<ArgumentException>(() => enrichmentService.SaveInstitutionEnrichment(model, instCode, email));
        }
        [Test]
        [TestCase("eqweqw", "qweqweq")]
        public void Test_GetInstitutionEnrichment_should_error(string instCode, string email)
        {
            var enrichmentService = new EnrichmentService(Context);

            Assert.Throws<InvalidOperationException>(() => enrichmentService.GetInstitutionEnrichment(instCode, email));
        }
        [Test]
        [TestCase("eqweqw", "qweqweq")]
        public void Test_PublishInstitutionEnrichment_should_return_invalid_operation_exception(string instCode, string email)
        {
            var enrichmentService = new EnrichmentService(Context);

            Assert.Throws<InvalidOperationException>(() => enrichmentService.PublishInstitutionEnrichment(instCode, email));
        }
        [Test]
        [TestCase("", "")]
        [TestCase(null, null)]
        public void Test_PublishInstitutionEnrichment_should_argument_exception(string instCode, string email)
        {
            var enrichmentService = new EnrichmentService(Context);
            Assert.Throws<ArgumentException>(() => enrichmentService.PublishInstitutionEnrichment(instCode, email));
        }
        [Test]
        public void Test_PublishInstitutionEnrichment_should_return_false()
        {
            Context.InstitutionEnrichments.RemoveRange(Context.InstitutionEnrichments);
            Context.Save();

            var enrichmentService = new EnrichmentService(Context);
            var publishResults = enrichmentService.PublishInstitutionEnrichment(ProviderInstCode.ToLower(), Email);
            publishResults.Should().BeFalse();

        }
    }
}
