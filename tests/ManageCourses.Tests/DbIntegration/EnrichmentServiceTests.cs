using System;
using System.Collections.Generic;
using System.Text;
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
        private const string _instCode = "school1";
        private const string _instDesc = "school1 description";
        private const string _email = "12345@example.org";

        protected override void Setup()
        {
            Context.UcasCourses.RemoveRange(Context.UcasCourses);
            Context.CourseCodes.RemoveRange(Context.CourseCodes);
            Context.UcasSubjects.RemoveRange(Context.UcasSubjects);
            Context.UcasCourseSubjects.RemoveRange(Context.UcasCourseSubjects);
            Context.UcasCampuses.RemoveRange(Context.UcasCampuses);
            Context.UcasCourseNotes.RemoveRange(Context.UcasCourseNotes);
            Context.UcasNoteTexts.RemoveRange(Context.UcasNoteTexts);
            Context.McOrganisationIntitutions.RemoveRange(Context.McOrganisationIntitutions);
            Context.UcasInstitutions.RemoveRange(Context.UcasInstitutions);
            Context.McOrganisations.RemoveRange(Context.McOrganisations);
            Context.McOrganisationUsers.RemoveRange(Context.McOrganisationUsers);
            Context.McUsers.RemoveRange(Context.McUsers);
            Context.InstitutionEnrichments.RemoveRange(Context.InstitutionEnrichments);
            Context.Save();
 
            var accreditingInstitution = new UcasInstitution
            {
                InstName = "Trilby University", // Universities can accredit courses provided by schools / SCITTs
                InstCode = _instCode,
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
                        AccreditingProvider = _instCode
                    }
                }
            };
            Context.Add(_ucasInstitution);

            var user = new McUser
            {
                Email = _email,
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

        [Test]
        public void Test_GetInstitutionEnrichment_should_not_be_null()
        {
            var enrichmentService = new EnrichmentService(Context);
            var result = enrichmentService.GetInstitutionEnrichment(_email, _instCode);

            result.Should().NotBeNull();
            result.EnrichmentModel.AccreditingProviderEnrichments.Should().BeEmpty();
        }
        [Test]
        public void Test_GetInstitutionEnrichment_should_return_results()
        {
            var instCode = "123";
            var enrichmentService = new EnrichmentService(Context);
            var result = enrichmentService.GetInstitutionEnrichment(_email, instCode);

            result.Should().BeNull();
        }
        [Test]
        public void Test_SaveInstitutionEnrichment_should_not_error()
        {
            const string trainWithDisabilityText = "TrainWithDisabilily Text";
            const string trainWithUsText = "TrainWithUs Text";

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
                            UcasInstitutionCode = _instCode,
                            Description = _instDesc
                        }
                    }
                }
            };
            
            enrichmentService.SaveInstitutionEnrichment(model, _instCode, _email);

        }
        //[Test]
        //public void Test_SaveInstitutionEnrichment_should_error()
        //{
        //    var instCode = "123";
        //    var enrichmentService = new EnrichmentService(Context);
        //    var model = new UcasInstitutionEnrichmentPostModel();
            
        //    Assert.That(() => enrichmentService.SaveInstitutionEnrichment(model, instCode),
        //        Throws.TypeOf<Exception>());
        //            //.With.Message.EqualTo("The HTTP status code of the response was not expected (404)."));
        //}

    }
}
