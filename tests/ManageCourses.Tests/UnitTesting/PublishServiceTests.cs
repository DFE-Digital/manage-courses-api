using System.Collections.Generic;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Services.Publish;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.SearchAndCompare.Domain.Client;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    public class PublishServiceTests
    {
        private Mock<IEnrichmentService> _enrichmentServiceMock;
        private Mock<IDataService> _dataServiceMock;
        private IPublishService _publishService;
        private Mock<ISearchAndCompareApi> _searchAndCompareApiMock;
        private const string InstitutionCode = "123";
        private const string CourseCode = "234";

        [SetUp]
        public void SetUp()
        {
            _enrichmentServiceMock = new Mock<IEnrichmentService>();
            _searchAndCompareApiMock = new Mock<ISearchAndCompareApi>();
            _dataServiceMock = new Mock<IDataService>();

            _publishService = new PublishService(_searchAndCompareApiMock.Object, new CourseMapper(), _dataServiceMock.Object, _enrichmentServiceMock.Object);
        }

        [Test]
        public void PublishEnrichedCourseWithEmailHappyPathTest()
        {
            var email = "tester@example.com";
            _dataServiceMock.Setup(x => x.GetUcasInstitutionForUser(email, InstitutionCode)).Returns(new UcasInstitution
            {
                AccreditedUcasCourses =
                    new List<UcasCourse> { new UcasCourse { InstCode = InstitutionCode, CrseCode = CourseCode } }
            });
            _dataServiceMock.Setup(x => x.GetCourse(email, InstitutionCode, CourseCode))
                .Returns(new Course { CourseCode = CourseCode, InstCode = InstitutionCode });

            _enrichmentServiceMock.Setup(x => x.GetInstitutionEnrichment(InstitutionCode, email))
                .Returns(new UcasInstitutionEnrichmentGetModel());
            _enrichmentServiceMock.Setup(x => x.GetCourseEnrichment(InstitutionCode, CourseCode, email))
                .Returns(new UcasCourseEnrichmentGetModel { CourseCode = CourseCode, InstCode = InstitutionCode });
            _searchAndCompareApiMock.Setup(x => x.SaveCoursesAsync(It.IsAny<List<SearchAndCompare.Domain.Models.Course>>())).ReturnsAsync(true);

            var result = _publishService.SaveSingleCourseToSearchAndCompare(InstitutionCode, CourseCode, email).Result;

            result.Should().BeTrue();
        }
        [Test]
        public void PublishBasicCourseWithEmailHappyPathTest()
        {
            var email = "tester@example.com";
            _dataServiceMock.Setup(x => x.GetUcasInstitutionForUser(email, InstitutionCode)).Returns(new UcasInstitution
            {
                AccreditedUcasCourses =
                    new List<UcasCourse> { new UcasCourse { InstCode = InstitutionCode, CrseCode = CourseCode } }
            });
            _dataServiceMock.Setup(x => x.GetCourse(email, InstitutionCode, CourseCode))
                .Returns(new Course { CourseCode = CourseCode, InstCode = InstitutionCode });

            _searchAndCompareApiMock.Setup(x => x.SaveCoursesAsync(It.IsAny<List<SearchAndCompare.Domain.Models.Course>>())).ReturnsAsync(true);

            var result = _publishService.SaveSingleCourseToSearchAndCompare(InstitutionCode, CourseCode, email).Result;

            result.Should().BeTrue();
        }

        [TestCase("", "", "")]
        [TestCase("123", "", "")]
        [TestCase("", "234", "")]
        [TestCase("", "", "email@qwe.com")]
        [TestCase("  ", "      ", "        ")]
        [TestCase("123  ", "      ", "        ")]
        [TestCase("  ", "234   ", "        ")]
        [TestCase("  ", "      ", "email@qwe.com")]
        [TestCase(null, null, null)]
        [TestCase("123", null, null)]
        [TestCase(null, "234", null)]
        [TestCase(null, null, "email@qwe.com")]
        public void PublishCourseWithEmailInvalidParametersTest(string instCode, string courseCode, string email)
        {
            var result = _publishService.SaveSingleCourseToSearchAndCompare(instCode, courseCode, email).Result;

            result.Should().BeFalse();
        }
    }
}
