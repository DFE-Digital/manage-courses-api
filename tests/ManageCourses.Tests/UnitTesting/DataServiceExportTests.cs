using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Api.Services.Data;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Tests.UnitTesting.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    class DataServiceExportTests
    {
        private ManageCoursesDbContext _dbContext;
        private DataService _dataService;

        [OneTimeSetUp]
        public void Setup()
        {
            _dbContext = TestHelper.GetFakeData(EnumTestType.DataService);
            _dataService = new DataService(_dbContext, new UserDataHelper(), new Mock<ILogger<DataService>>().Object);
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_email_should_return_loaded_object()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.OrgWithProviderEmail, TestHelper.OrgUcasCodeWithProviders);

            Assert.True(result.ProviderCourses.Count == 3);
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_lower_case_ucas_code_should_return_loaded_object()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.OrgWithProviderEmail, TestHelper.OrgUcasCodeWithProviders.ToLower());

            Assert.True(result.ProviderCourses.Count == 3);
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_upper_case_ucas_code_should_return_loaded_object()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.OrgWithProviderEmail, TestHelper.OrgUcasCodeWithProviders.ToUpper());

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
            var result = _dataService.GetCoursesForUserOrganisation(email, TestHelper.OrgUcasCodeWithProviders);

            Assert.True(result.ProviderCourses.Count == 0);
        }
        [Test]
        public void GetCoursesForUserOrganisation_should_return_providers()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.OrgWithProviderEmail, TestHelper.OrgUcasCodeWithProviders);

            Assert.True(result.ProviderCourses.All(x => !string.IsNullOrWhiteSpace(x.AccreditingProviderId)));
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_lower_case_ucas_code_should_return_providers()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.OrgWithProviderEmail, TestHelper.OrgUcasCodeWithProviders.ToLower());

            Assert.True(result.ProviderCourses.All(x => !string.IsNullOrWhiteSpace(x.AccreditingProviderId)));
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_upper_case_ucas_code_should_return_providers()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.OrgWithProviderEmail, TestHelper.OrgUcasCodeWithProviders.ToUpper());

            Assert.True(result.ProviderCourses.All(x => !string.IsNullOrWhiteSpace(x.AccreditingProviderId)));
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_lower_case_ucas_code_should_return_course_details()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.OrgWithProviderEmail, TestHelper.OrgUcasCodeWithProviders.ToLower());

            Assert.True(result.ProviderCourses.Select(CheckCourseDetails).All(y => y));
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_upper_case_ucas_code_should_return_course_details()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.OrgWithProviderEmail, TestHelper.OrgUcasCodeWithProviders.ToUpper());

            Assert.True(result.ProviderCourses.Select(CheckCourseDetails).All(y => y));
        }
        [Test]
        public void GetCoursesForUserOrganisation_should_return_course_details()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.OrgWithProviderEmail, TestHelper.OrgUcasCodeWithProviders);

            Assert.True(result.ProviderCourses.Select(CheckCourseDetails).All(y => y));
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_lower_case_ucas_code_should_return_course_variants()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.OrgWithProviderEmail, TestHelper.OrgUcasCodeWithProviders.ToLower());

            //use multiple asserts rather then the flattened assert above as this is easier to debug
            Assert.True(CheckVariants(result.ProviderCourses[0].CourseDetails[0]));
            Assert.True(CheckVariants(result.ProviderCourses[1].CourseDetails[0]));
            Assert.True(CheckVariants(result.ProviderCourses[2].CourseDetails[0]));
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_upper_case_ucas_code_should_return_course_variants()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.OrgWithProviderEmail, TestHelper.OrgUcasCodeWithProviders.ToUpper());

            //use multiple asserts rather then the flattened assert above as this is easier to debug
            Assert.True(CheckVariants(result.ProviderCourses[0].CourseDetails[0]));
            Assert.True(CheckVariants(result.ProviderCourses[1].CourseDetails[0]));
            Assert.True(CheckVariants(result.ProviderCourses[2].CourseDetails[0]));
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_lower_case_ucas_code_should_return_campuses()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.OrgWithProviderEmail, TestHelper.OrgUcasCodeWithProviders.ToLower());

            Assert.True(CheckCampuses(result.ProviderCourses[0].CourseDetails[0].Variants[0]));
            Assert.True(CheckCampuses(result.ProviderCourses[1].CourseDetails[0].Variants[0]));
            Assert.True(CheckCampuses(result.ProviderCourses[2].CourseDetails[0].Variants[0]));
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_upper_case_ucas_code_should_return_campuses()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.OrgWithProviderEmail, TestHelper.OrgUcasCodeWithProviders.ToUpper());

            Assert.True(CheckCampuses(result.ProviderCourses[0].CourseDetails[0].Variants[0]));
            Assert.True(CheckCampuses(result.ProviderCourses[1].CourseDetails[0].Variants[0]));
            Assert.True(CheckCampuses(result.ProviderCourses[2].CourseDetails[0].Variants[0]));
        }
        [Test]
        public void GetCoursesForUserOrganisation_should_not_return_providers()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.OrgWithNoProviderEmail, TestHelper.OrgUcasCodeNoProviders);

            Assert.True(result.ProviderCourses.Count == 1);
            Assert.True(string.IsNullOrWhiteSpace(result.ProviderCourses[0].AccreditingProviderId));
            Assert.True(result.ProviderCourses[0].CourseDetails.Count == 19);
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_no_providers_and_lower_case_ucas_code_should_return_course_variants()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.OrgWithNoProviderEmail, TestHelper.OrgUcasCodeNoProviders.ToLower());

            Assert.True(CheckVariants(result.ProviderCourses[0].CourseDetails[0]));
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_no_providers_and_upper_case_ucas_code_should_return_course_variants()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.OrgWithNoProviderEmail, TestHelper.OrgUcasCodeNoProviders.ToUpper());

            Assert.True(CheckVariants(result.ProviderCourses[0].CourseDetails[0]));
        }
        [Test]
        public void GetCoursesForUserOrganisation_with_no_providers_should_return_course_variants()
        {
            var result = _dataService.GetCoursesForUserOrganisation(TestHelper.OrgWithNoProviderEmail, TestHelper.OrgUcasCodeNoProviders);

            Assert.True(CheckVariants(result.ProviderCourses[0].CourseDetails[0]));
        }

        #region Data Checks
        private bool CheckCourseDetails(ProviderCourse course)
        {
            var returnBool = course.CourseDetails.Any(x => (!string.IsNullOrWhiteSpace(x.CourseTitle)) && (!string.IsNullOrWhiteSpace(x.AgeRange)));

            return returnBool;
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
