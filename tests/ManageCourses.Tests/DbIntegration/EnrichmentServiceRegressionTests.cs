using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Tests.TestUtilities;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.DbIntegration
{
    /// <summary>
    ///
    /// This test clas contains one test case (which would have exposed a bug when getting an enrichment record)
    /// An error was thrown when a different user to the origin 'SavedBy' user tried to get the record.
    /// This was because the SavedBy user and UpdateBy user objects were null in the result.
    /// In order to re-create the bug I had to re-instate the old code and watch the test fail.
    /// However this wasn't easy to re-create as the context was still holding on the the 2 users (which need to be setup for the same oranisation)
    /// The answer was to initialise the database in the test setup with the enrichment reacords as well as the users/organisations etc.
    /// Then refreshing the context before the query was run.
    /// </summary>
    [TestFixture]
    [Category("Integration")]
    [Category("Integration_DB")]
    [Explicit]
    public class EnrichmentServiceRegressionTests : DbIntegrationTestBase
    {
        private UcasInstitution _ucasInstitution;
        private const string ProviderInstCode = "HNY1";
        private const string AccreditingInstCode = "TRILU";

        private const string Email = "12345@example.org";
        private const string Email2 = "2345678@example.org";
        private const string TrainWithDisabilityText = "TrainWithDisabilily Text";
        private const string TrainWithUsText = "TrainWithUs Text";

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
            var user2 = new McUser
            {
                Email = Email2,
            };
            Context.Add(user);
            //create the organisation and add user1 and user2 to it
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
                    new McOrganisationUser
                    {
                        McUser = user2,
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
            //now add the enrichment model with user1
            var enrichmentModel = new InstitutionEnrichmentModel
            {
                TrainWithDisability = TrainWithDisabilityText,
                TrainWithUs = TrainWithUsText,
                AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>
                {
                    new AccreditingProviderEnrichment
                    {
                        UcasInstitutionCode = AccreditingInstCode,
                        Description = "xcvxcvxcv",
                    }
                }
            };
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
            var content = JsonConvert.SerializeObject(enrichmentModel, jsonSerializerSettings);

            var enrichment = new InstitutionEnrichment
            {
                InstCode = ProviderInstCode,
                CreatedTimestampUtc = DateTime.UtcNow,
                UpdatedTimestampUtc = DateTime.UtcNow,
                LastPublishedTimestampUtc = null,
                CreatedByUser = user,
                UpdatedByUser = user,
                Status = EnumStatus.Draft,
                JsonData = content,
            };
            Context.InstitutionEnrichments.Add(enrichment);
            Context.SaveChanges();
            Context = ContextLoader.GetDbContext(Config);//refresh the context
        }

        /// <summary>
        /// This test breaks when the old code in the enrichment service is re-instated
        /// </summary>
        [Test]
        public void TestGetInstitutionEnrichmentWithDifferentUserFromSavedUser()
        {
            //get a new context            
            var enrichmentService = new EnrichmentService(Context);
            //test get the enrichment using user2
            var result = enrichmentService.GetInstitutionEnrichment(ProviderInstCode, Email2);
            result.Should().NotBeNull();
            result.EnrichmentModel.Should().NotBeNull();
            result.EnrichmentModel.TrainWithDisability.Should().BeEquivalentTo(TrainWithDisabilityText);
            result.EnrichmentModel.TrainWithUs.Should().BeEquivalentTo(TrainWithUsText);
            result.LastPublishedTimestampUtc.Should().BeNull();
            result.Status.Should().BeEquivalentTo(EnumStatus.Draft);
        }
    }
}
