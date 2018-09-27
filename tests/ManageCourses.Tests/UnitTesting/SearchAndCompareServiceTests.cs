using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Services.Publish;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.SearchAndCompare.Domain.Client;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    public class SearchAndCompareServiceTests
    {
        private Mock<IEnrichmentService> _enrichmentServiceMock;
        private Mock<IDataService> _dataServiceMock;
        private ISearchAndCompareService _searchAndCompareService;
        private Mock<IHttpClient> _httpMock;
        private Mock<ILogger<SearchAndCompareService>> _loggerMock;
        private const string InstitutionCode = "123";
        private const string CourseCode = "234";

        private const string sncUrl = "https://api.example.com";

        [SetUp]
        public void SetUp()
        {
            _enrichmentServiceMock = new Mock<IEnrichmentService>();

            _dataServiceMock = new Mock<IDataService>();

            _loggerMock = new Mock<ILogger<SearchAndCompareService>>();
            _httpMock = new Mock<IHttpClient>();
            var searchAndCompareApi = new SearchAndCompareApi(_httpMock.Object, sncUrl);

            _searchAndCompareService = new SearchAndCompareService(searchAndCompareApi, new CourseMapper(), _dataServiceMock.Object, _enrichmentServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public void PublishEnrichedCourseWithEmailHappyPathTest()
        {
            var email = "tester@example.com";
            _dataServiceMock.Setup(x => x.GetUcasInstitutionForUser(email, InstitutionCode)).Returns(new UcasInstitution
            {
                InstCode = InstitutionCode,
                AccreditedUcasCourses =
                    new List<UcasCourse> { new UcasCourse { InstCode = InstitutionCode, CrseCode = CourseCode, ProgramType = "SD", CrseTitle = "History"} }
            });
            _dataServiceMock.Setup(x => x.GetCourse(email, InstitutionCode, CourseCode))
                .Returns(new Course { CourseCode = CourseCode, InstCode = InstitutionCode, ProgramType = "SD", Subjects = "History", Name = "History"});

            _enrichmentServiceMock.Setup(x => x.GetInstitutionEnrichment(InstitutionCode, email, true))
                .Returns(new UcasInstitutionEnrichmentGetModel{EnrichmentModel = new InstitutionEnrichmentModel()});

            var enrichmentModel = new CourseEnrichmentModel {FeeDetails = "It's gonna cost you", FeeInternational = (Decimal)123.5, FeeUkEu = (Decimal)234.5};
            _enrichmentServiceMock.Setup(x => x.GetCourseEnrichment(InstitutionCode, CourseCode, email))
                .Returns(new UcasCourseEnrichmentGetModel { CourseCode = CourseCode, InstCode = InstitutionCode, EnrichmentModel = enrichmentModel, Status = EnumStatus.Published});
            _httpMock.Setup(x => x.PostAsync(It.Is<Uri>(y => y.AbsoluteUri == $"{sncUrl}/courses/{InstitutionCode}/{CourseCode}"), It.IsAny<StringContent>())).ReturnsAsync(
                new HttpResponseMessage() {
                    StatusCode = HttpStatusCode.OK
            }
            ).Verifiable();

            var result = _searchAndCompareService.SaveSingleCourseToSearchAndCompare(InstitutionCode, CourseCode, email).Result;

            result.Should().BeTrue();
            _httpMock.VerifyAll();
        }
        [Test]
        public void PublishEnrichedCourseWithEmailDraftTest()
        {
            var email = "tester@example.com";
            _dataServiceMock.Setup(x => x.GetUcasInstitutionForUser(email, InstitutionCode)).Returns(new UcasInstitution
            {
                InstCode = InstitutionCode,
                AccreditedUcasCourses =
                    new List<UcasCourse> { new UcasCourse { InstCode = InstitutionCode, CrseCode = CourseCode, ProgramType = "SD", CrseTitle = "History" } }
            });
            _dataServiceMock.Setup(x => x.GetCourse(email, InstitutionCode, CourseCode))
                .Returns(new Course { CourseCode = CourseCode, InstCode = InstitutionCode, ProgramType = "SD", Subjects = "History", Name = "History" });

            _enrichmentServiceMock.Setup(x => x.GetInstitutionEnrichment(InstitutionCode, email, true))
                .Returns(new UcasInstitutionEnrichmentGetModel { EnrichmentModel = new InstitutionEnrichmentModel() });

            var enrichmentModel = new CourseEnrichmentModel { FeeDetails = "It's gonna cost you", FeeInternational = (Decimal)123.5, FeeUkEu = (Decimal)234.5 };
            _enrichmentServiceMock.Setup(x => x.GetCourseEnrichment(InstitutionCode, CourseCode, email))
                .Returns(new UcasCourseEnrichmentGetModel { CourseCode = CourseCode, InstCode = InstitutionCode, EnrichmentModel = enrichmentModel, Status = EnumStatus.Draft });
            var result = _searchAndCompareService.SaveSingleCourseToSearchAndCompare(InstitutionCode, CourseCode, email).Result;

            result.Should().BeFalse();
        }

        [Test][Ignore("This test will fail atm. Revisit when we start publishing basic courses from here")]
        public void PublishBasicCourseWithEmailHappyPathTest()
        {
            var email = "tester@example.com";
            _dataServiceMock.Setup(x => x.GetUcasInstitutionForUser(email, InstitutionCode)).Returns(new UcasInstitution
            {
                InstCode = InstitutionCode,
                AccreditedUcasCourses =
                    new List<UcasCourse> { new UcasCourse { InstCode = InstitutionCode, CrseCode = CourseCode, ProgramType = "SD", CrseTitle = "History" } }
            });
            _dataServiceMock.Setup(x => x.GetCourse(email, InstitutionCode, CourseCode))
                .Returns(new Course { CourseCode = CourseCode, InstCode = InstitutionCode, ProgramType = "SD", Subjects = "History", Name = "History" });

            _enrichmentServiceMock.Setup(x => x.GetInstitutionEnrichment(InstitutionCode, email, true))
                .Returns(new UcasInstitutionEnrichmentGetModel());

            _httpMock.Setup(x => x.PostAsync(It.Is<Uri>(y => y.AbsoluteUri == $"{sncUrl}/courses/{InstitutionCode}/{CourseCode}"), It.IsAny<StringContent>())).ReturnsAsync(
                new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK
                }
            ).Verifiable();

            var result = _searchAndCompareService.SaveSingleCourseToSearchAndCompare(InstitutionCode, CourseCode, email).Result;

            result.Should().BeTrue();
            _httpMock.VerifyAll();
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
            var result = _searchAndCompareService.SaveSingleCourseToSearchAndCompare(instCode, courseCode, email).Result;

            result.Should().BeFalse();
        }
    }
}
