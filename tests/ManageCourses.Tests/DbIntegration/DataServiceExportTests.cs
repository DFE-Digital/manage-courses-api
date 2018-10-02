using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Services.Data;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Tests.UnitTesting.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.DbIntegration
{
    [TestFixture]
    class DataServiceExportTests : DbIntegrationTestBase
    {
        private DataService _dataService;

        protected override void Setup()
        {
            new TestHelper(Context).BuildFakeDataForService();
            var mockEnrichmentService = new Mock<IEnrichmentService>();
            var mockPdgeWhitelist = new Mock<IPgdeWhitelist>();
            mockPdgeWhitelist.Setup(x => x.ForInstitution(It.IsAny<string>())).Returns(new List<PgdeCourse>());
            _dataService = new DataService(Context, mockEnrichmentService.Object, new UserDataHelper(), new Mock<ILogger<DataService>>().Object, mockPdgeWhitelist.Object);
        }

        [Test]
        public void GetCoursesForUserOrganisation_with_email_should_return_loaded_object()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.EmailWithProvider, TestHelper.UcasInstCodeWithProviders);

            Assert.True(result.ProviderCourses.Count == 3);
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_lower_case_ucas_code_should_return_loaded_object()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.EmailWithProvider, TestHelper.UcasInstCodeWithProviders.ToLower());

            Assert.True(result.ProviderCourses.Count == 3);
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_upper_case_ucas_code_should_return_loaded_object()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.EmailWithProvider, TestHelper.UcasInstCodeWithProviders.ToUpper());

            Assert.True(result.ProviderCourses.Count == 3);
        }
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase("qwpeoiqwepoi")]
        [TestCase("idontexist@nowhere.com")]
        public void GetCoursesForUserOrganisation_should_return_empty_object(string email)
        {
            var result = _dataService.GetCoursesForUserOrganisation(email, TestHelper.UcasInstCodeWithProviders);

            Assert.True(result.ProviderCourses.Count == 0);
        }
        [Test]
        public void GetCoursesForUserOrganisation_should_return_providers()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.EmailWithProvider, TestHelper.UcasInstCodeWithProviders);

            Assert.True(result.ProviderCourses.All(x => !string.IsNullOrWhiteSpace(x.AccreditingProviderId)));
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_lower_case_ucas_code_should_return_providers()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.EmailWithProvider, TestHelper.UcasInstCodeWithProviders.ToLower());

            Assert.True(result.ProviderCourses.All(x => !string.IsNullOrWhiteSpace(x.AccreditingProviderId)));
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_upper_case_ucas_code_should_return_providers()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.EmailWithProvider, TestHelper.UcasInstCodeWithProviders.ToUpper());

            Assert.True(result.ProviderCourses.All(x => !string.IsNullOrWhiteSpace(x.AccreditingProviderId)));
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_lower_case_ucas_code_should_return_course_details()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.EmailWithProvider, TestHelper.UcasInstCodeWithProviders.ToLower());

            foreach (var providerCourse in result.ProviderCourses)
            {
                CheckCourseDetails(providerCourse);
            }
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_upper_case_ucas_code_should_return_course_details()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.EmailWithProvider, TestHelper.UcasInstCodeWithProviders.ToUpper());

            foreach (var providerCourse in result.ProviderCourses)
            {
                CheckCourseDetails(providerCourse);
            }
        }
        [Test]
        public void GetCoursesForUserOrganisation_should_return_course_details()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.EmailWithProvider, TestHelper.UcasInstCodeWithProviders);

            foreach (var providerCourse in result.ProviderCourses)
            {
                CheckCourseDetails(providerCourse);
            }
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_lower_case_ucas_code_should_return_course_variants()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.EmailWithProvider, TestHelper.UcasInstCodeWithProviders.ToLower());

            //use multiple asserts rather then the flattened assert above as this is easier to debug
            Assert.True(CheckVariants(result.ProviderCourses[0].CourseDetails[0]));
            Assert.True(CheckVariants(result.ProviderCourses[1].CourseDetails[0]));
            Assert.True(CheckVariants(result.ProviderCourses[2].CourseDetails[0]));
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_upper_case_ucas_code_should_return_course_variants()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.EmailWithProvider, TestHelper.UcasInstCodeWithProviders.ToUpper());

            //use multiple asserts rather then the flattened assert above as this is easier to debug
            Assert.True(CheckVariants(result.ProviderCourses[0].CourseDetails[0]));
            Assert.True(CheckVariants(result.ProviderCourses[1].CourseDetails[0]));
            Assert.True(CheckVariants(result.ProviderCourses[2].CourseDetails[0]));
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_lower_case_ucas_code_should_return_campuses()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.EmailWithProvider, TestHelper.UcasInstCodeWithProviders.ToLower());

            Assert.True(CheckCampuses(result.ProviderCourses[0].CourseDetails[0].Variants[0]));
            Assert.True(CheckCampuses(result.ProviderCourses[1].CourseDetails[0].Variants[0]));
            Assert.True(CheckCampuses(result.ProviderCourses[2].CourseDetails[0].Variants[0]));
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_upper_case_ucas_code_should_return_campuses()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.EmailWithProvider, TestHelper.UcasInstCodeWithProviders.ToUpper());

            Assert.True(CheckCampuses(result.ProviderCourses[0].CourseDetails[0].Variants[0]));
            Assert.True(CheckCampuses(result.ProviderCourses[1].CourseDetails[0].Variants[0]));
            Assert.True(CheckCampuses(result.ProviderCourses[2].CourseDetails[0].Variants[0]));
        }
        [Test]
        public void GetCoursesForUserOrganisation_should_not_return_providers()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.EmailNoProvider, TestHelper.UcasInstCodeNoProviders);

            result.ProviderCourses.Count.Should().Be(1);
            result.ProviderCourses[0].AccreditingProviderId.Should().BeNullOrWhiteSpace();
            result.ProviderCourses[0].CourseDetails.Count.Should().Be(19);
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_no_providers_and_lower_case_ucas_code_should_return_course_variants()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.EmailNoProvider, TestHelper.UcasInstCodeNoProviders.ToLower());

            Assert.True(CheckVariants(result.ProviderCourses[0].CourseDetails[0]));
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_no_providers_and_upper_case_ucas_code_should_return_course_variants()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.EmailNoProvider, TestHelper.UcasInstCodeNoProviders.ToUpper());

            Assert.True(CheckVariants(result.ProviderCourses[0].CourseDetails[0]));
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_no_providers_should_return_course_variants()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.EmailNoProvider, TestHelper.UcasInstCodeNoProviders);

            Assert.True(CheckVariants(result.ProviderCourses[0].CourseDetails[0]));
        }

        #region Data Checks
        private void CheckCourseDetails(ProviderCourse course)
        {
            course.CourseDetails.Any(x => (!string.IsNullOrWhiteSpace(x.CourseTitle)) && (!string.IsNullOrWhiteSpace(x.AgeRange))).Should().BeTrue();
        }

        private bool CheckVariants(CourseDetail courseDetail)
        {
            var returnBool = courseDetail.Variants.Any(x =>
                (!string.IsNullOrWhiteSpace(x.UcasCode)) && (!string.IsNullOrWhiteSpace(x.CourseCode)) &&
                (!string.IsNullOrWhiteSpace(x.Name)));

            return returnBool;
        }
        private bool CheckCampuses(CourseVariant variant)
        {
            var returnBool = variant.Campuses.Any(x =>
                (!string.IsNullOrWhiteSpace(x.Name)) && (!string.IsNullOrWhiteSpace(x.Code)));

            return returnBool;
        }

        private bool CheckForDuplicateOrganisations(IEnumerable<UserOrganisation> organisations)
        {
            var returnBool = false;
            var userOrganisations = organisations.ToList();
            foreach (var organisation in userOrganisations)
            {
                var duplicateOrgs = userOrganisations.Where(o => o.UcasCode == organisation.UcasCode && o.OrganisationId == organisation.OrganisationId &&
                    o.OrganisationName == organisation.OrganisationName).ToList();
                if (duplicateOrgs.Count <= 1) continue;

                returnBool = true;
                break;
            }

            return returnBool;
        }
        #endregion
    }
}
