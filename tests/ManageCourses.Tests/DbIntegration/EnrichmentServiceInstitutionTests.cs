using System;
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
    [Explicit]
    public class EnrichmentServiceInstitutionTests : DbIntegrationTestBase
    {
        private Provider _ucasInstitution;
        private const string ProviderInstCode = "HNY1";
        private const string AccreditingInstCode = "TRILU";

        private const string Email = "12345@example.org";

        protected override void Setup()
        {
            var accreditingInstitution = new Provider
            {
                ProviderName = "Trilby University", // Universities can accredit courses provided by schools / SCITTs
                ProviderCode = AccreditingInstCode,
            };
            Context.Add(accreditingInstitution);

            const string providerInstCode = "HNY1";
            const string crseCode = "TK101";
            _ucasInstitution = new Provider
            {
                ProviderName = "Honey Lane School", // This is a school so has to have a university accredit the courses it offers
                ProviderCode = providerInstCode,
                Courses = new List<Course>
                {
                    new Course
                    {
                        CourseCode = crseCode,
                        Name = "Conscious control of telekenisis",
                        AccreditingProvider = accreditingInstitution,
                    }
                }
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
        public void Test_InstitutionEnrichment_And_Publishing_Workflow()
        {
            const string trainWithDisabilityText = "TrainWithDisabilily Text";
            const string trainWithUsText = "TrainWithUs Text";
            const string trainWithDisabilityUpdatedText = "TrainWithDisabilily Text updated";
            const string trainWithUsUpdatedText = "TrainWithUs Text updated";
            const string instDesc = "school1 description enrichement";

            var enrichmentService = new EnrichmentService(Context);
            var model = new UcasProviderEnrichmentPostModel
            {
                EnrichmentModel = new ProviderEnrichmentModel
                {
                    TrainWithDisability = trainWithDisabilityText,
                    TrainWithUs = trainWithUsText,
                    AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>
                    {
                        new AccreditingProviderEnrichment
                        {
                            UcasProviderCode = AccreditingInstCode,
                            Description = instDesc,
                        }
                    }
                }
            };

            var emptyEnrichment = enrichmentService.GetProviderEnrichment(ProviderInstCode, Email);
            emptyEnrichment.Status.Should().Be(EnumStatus.Draft, "no enrichments saved yet");

            //test save
            enrichmentService.SaveProviderEnrichment(model, ProviderInstCode.ToLower(), Email);

            //test get
            var result = enrichmentService.GetProviderEnrichment(ProviderInstCode.ToLower(), Email);

            result.Should().NotBeNull();
            result.EnrichmentModel.Should().NotBeNull();
            result.EnrichmentModel.TrainWithDisability.Should().BeEquivalentTo(trainWithDisabilityText);
            result.EnrichmentModel.TrainWithUs.Should().BeEquivalentTo(trainWithUsText);
            result.LastPublishedTimestampUtc.Should().BeNull();
            result.Status.Should().BeEquivalentTo(EnumStatus.Draft);

            //test update
            var updatedmodel = new UcasProviderEnrichmentPostModel
            {
                EnrichmentModel = new ProviderEnrichmentModel
                {
                    TrainWithDisability = trainWithDisabilityUpdatedText,
                    TrainWithUs = trainWithUsUpdatedText,
                    AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>
                    {
                        new AccreditingProviderEnrichment
                        {
                            UcasProviderCode = AccreditingInstCode,
                            Description = instDesc,
                        }
                    }
                }
            };

            enrichmentService.SaveProviderEnrichment(updatedmodel, ProviderInstCode.ToLower(), Email);
            var updateResult = enrichmentService.GetProviderEnrichment(ProviderInstCode, Email);
            updateResult.EnrichmentModel.TrainWithDisability.Should().BeEquivalentTo(trainWithDisabilityUpdatedText);
            updateResult.EnrichmentModel.TrainWithUs.Should().BeEquivalentTo(trainWithUsUpdatedText);
            updateResult.LastPublishedTimestampUtc.Should().BeNull();
            //publish
            var publishResults = enrichmentService.PublishProviderEnrichment(ProviderInstCode.ToLower(), Email);
            publishResults.Should().BeTrue();
            var publishRecord = enrichmentService.GetProviderEnrichment(ProviderInstCode.ToLower(), Email);
            publishRecord.Status.Should().BeEquivalentTo(EnumStatus.Published);
            publishRecord.LastPublishedTimestampUtc.Should().NotBeNull();
            //test save again after publish
            enrichmentService.SaveProviderEnrichment(model, ProviderInstCode.ToLower(), Email);
            var updateResult2 = enrichmentService.GetProviderEnrichment(ProviderInstCode, Email);
            updateResult2.EnrichmentModel.TrainWithDisability.Should().BeEquivalentTo(trainWithDisabilityText);
            updateResult2.EnrichmentModel.TrainWithUs.Should().BeEquivalentTo(trainWithUsText);
            updateResult2.Status.Should().BeEquivalentTo(EnumStatus.Draft);
            updateResult2.LastPublishedTimestampUtc.ToString().Should().BeEquivalentTo(publishRecord.LastPublishedTimestampUtc.ToString());
            //check number of records generated from this
            var draftCount = Context.ProviderEnrichments.Count(x => x.Status == EnumStatus.Draft);
            var publishedCount = Context.ProviderEnrichments.Count(x => x.Status == EnumStatus.Published);
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
            var model = new UcasProviderEnrichmentPostModel
            {
                EnrichmentModel = new ProviderEnrichmentModel
                {
                    TrainWithDisability = trainWithDisabilityText,
                    TrainWithUs = trainWithUsText,
                    AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>
                    {
                        new AccreditingProviderEnrichment
                        {
                            UcasProviderCode = AccreditingInstCode,
                            Description = instDesc,
                        }
                    }
                }
            };

            Assert.Throws<InvalidOperationException>(() => enrichmentService.SaveProviderEnrichment(model, instCode, email));
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
            var model = new UcasProviderEnrichmentPostModel
            {
                EnrichmentModel = new ProviderEnrichmentModel
                {
                    TrainWithDisability = trainWithDisabilityText,
                    TrainWithUs = trainWithUsText,
                    AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>
                    {
                        new AccreditingProviderEnrichment
                        {
                            UcasProviderCode = AccreditingInstCode,
                            Description = instDesc,
                        }
                    }
                }
            };

            Assert.Throws<ArgumentException>(() => enrichmentService.SaveProviderEnrichment(model, instCode, email));
        }
        [Test]
        [TestCase("eqweqw", "qweqweq")]
        public void Test_GetInstitutionEnrichment_should_error(string instCode, string email)
        {
            var enrichmentService = new EnrichmentService(Context);

            Assert.Throws<InvalidOperationException>(() => enrichmentService.GetProviderEnrichment(instCode, email));
        }
        [Test]
        [TestCase("eqweqw", "qweqweq")]
        public void Test_PublishInstitutionEnrichment_should_return_invalid_operation_exception(string instCode, string email)
        {
            var enrichmentService = new EnrichmentService(Context);

            Assert.Throws<InvalidOperationException>(() => enrichmentService.PublishProviderEnrichment(instCode, email));
        }
        [Test]
        [TestCase("", "")]
        [TestCase(null, null)]
        public void Test_PublishInstitutionEnrichment_should_argument_exception(string instCode, string email)
        {
            var enrichmentService = new EnrichmentService(Context);
            Assert.Throws<ArgumentException>(() => enrichmentService.PublishProviderEnrichment(instCode, email));
        }
        [Test]
        public void Test_PublishInstitutionEnrichment_should_return_false()
        {
            Context.ProviderEnrichments.RemoveRange(Context.ProviderEnrichments);
            Context.Save();

            var enrichmentService = new EnrichmentService(Context);
            var publishResults = enrichmentService.PublishProviderEnrichment(ProviderInstCode.ToLower(), Email);
            publishResults.Should().BeFalse();

        }

        [Test]
        public void EnrichmentDataSurvivesDeleteAndRecreate()
        {
            // Arrange
            var enrichmentService = new EnrichmentService(Context);
            var dataService = new DataService(Context, enrichmentService, new Mock<ILogger<DataService>>().Object);
            var sourceModel = new UcasProviderEnrichmentPostModel
            {
                EnrichmentModel = new ProviderEnrichmentModel
                {
                    TrainWithUs = "Oh the grand old Duke of York",
                    TrainWithDisability = "He had 10,000 men",
                    AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>
                    {
                        new AccreditingProviderEnrichment
                        {
                            UcasProviderCode = AccreditingInstCode,
                            Description = "He marched them up to the top of the hill"
                        }
                    }
                },
            };
            enrichmentService.SaveProviderEnrichment(sourceModel, _ucasInstitution.ProviderCode, Email);

            // Act
            var ucasPayload = new UcasPayload
            {
                // todo: test with change of this institution: https://trello.com/c/e1FwXuYk/133-ucas-institutions-dont-get-updated-during-ucas-import
                Institutions = new List<UcasInstitution>
                {
                    new UcasInstitution
                    {
                        InstCode = _ucasInstitution.ProviderCode,
                        InstName = "Rebranded Provider",
                    },
                    new UcasInstitution
                    {
                        InstCode = AccreditingInstCode,
                        InstName = "Rebranded Accrediting Provider",
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
            var res = enrichmentService.GetProviderEnrichment(_ucasInstitution.ProviderCode, Email);
            res.EnrichmentModel.TrainWithUs.Should().Be(sourceModel.EnrichmentModel.TrainWithUs);
            res.EnrichmentModel.TrainWithDisability.Should().Be(sourceModel.EnrichmentModel.TrainWithDisability);
            res.EnrichmentModel.AccreditingProviderEnrichments.Should().HaveCount(1);
            res.EnrichmentModel.AccreditingProviderEnrichments.Should().HaveSameCount(sourceModel.EnrichmentModel.AccreditingProviderEnrichments);
            res.EnrichmentModel.AccreditingProviderEnrichments[0].Description.Should().Be(sourceModel.EnrichmentModel.AccreditingProviderEnrichments[0].Description);
            res.EnrichmentModel.AccreditingProviderEnrichments[0].UcasProviderCode.Should().Be(sourceModel.EnrichmentModel.AccreditingProviderEnrichments[0].UcasProviderCode);
        }

    }
}
