﻿using System;
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
    [Ignore("replaced with ManageCoursesBAckendService")]
    [Obsolete("replaced with ManageCoursesBAckendService")]

    public class SearchAndCompareServiceTests
    {
        private Mock<IEnrichmentService> _enrichmentServiceMock;
        private Mock<IDataService> _dataServiceMock;
        private ISearchAndCompareService _searchAndCompareService;
        private Mock<IHttpClient> _httpMock;
        private Mock<ILogger<SearchAndCompareService>> _loggerMock;
        private const string ProviderCode = "123";
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
            Provider provider = new Provider
            {
                ProviderCode = ProviderCode,
                AccreditedCourses =
                    new List<Course> { new Course { CourseCode = CourseCode, ProgramType = "SD", Name = "History" } }
            };
            _dataServiceMock.Setup(x => x.GetProviderForUser(email, ProviderCode)).Returns(provider);
            _dataServiceMock.Setup(x => x.GetCourseForUser(email, ProviderCode, CourseCode))
                .Returns(BuildCourse(provider));

            _dataServiceMock.Setup(x => x.GetCoursesForUser(email, ProviderCode))
                .Returns(new List<Course>{ new Course { CourseCode = CourseCode, Provider = provider, ProgramType = "SD", CourseSubjects = new List<CourseSubject> { new CourseSubject { Subject = new Subject { SubjectName = "History"}}}, Name = "History" } } );

            _enrichmentServiceMock.Setup(x => x.GetProviderEnrichmentForPublish(ProviderCode, email))
                .Returns(new UcasProviderEnrichmentGetModel{EnrichmentModel = new ProviderEnrichmentModel()});

            var enrichmentModel = new CourseEnrichmentModel {FeeDetails = "It's gonna cost you", FeeInternational = (Decimal)123.5, FeeUkEu = (Decimal)234.5};
            _enrichmentServiceMock.Setup(x => x.GetCourseEnrichmentForPublish(ProviderCode, CourseCode, email))
                .Returns(new UcasCourseEnrichmentGetModel { CourseCode = CourseCode, ProviderCode = ProviderCode, EnrichmentModel = enrichmentModel, Status = EnumStatus.Published});
            _httpMock.Setup(x => x.PutAsync(It.Is<Uri>(y => y.AbsoluteUri == $"{sncUrl}/courses"), It.IsAny<StringContent>())).ReturnsAsync(
                new HttpResponseMessage() {
                    StatusCode = HttpStatusCode.OK
            }
            ).Verifiable();

            var result = _searchAndCompareService.SaveCourse(ProviderCode, CourseCode, email).Result;

            result.Should().BeTrue();
            _httpMock.VerifyAll();
        }
        [Test]
        public void PublishEnrichedCoursesWithEmailHappyPathTest()
        {
            var email = "tester@example.com";
            var provider = new Provider
            {
                ProviderCode = ProviderCode,
                AccreditedCourses =
                    new List<Course> { new Course { CourseCode = CourseCode, ProgramType = "SD", Name = "History" } }
            };

            _dataServiceMock.Setup(x => x.GetProviderForUser(email, ProviderCode)).Returns(provider);
            _dataServiceMock.Setup(x => x.GetCourseForUser(email, ProviderCode, CourseCode))
                .Returns(BuildCourse(provider));

            _dataServiceMock.Setup(x => x.GetCourseForUser(email, ProviderCode, CourseCode + "1"))
                .Returns(new Course { CourseCode = CourseCode + "1", Provider = provider, ProgramType = "SD", CourseSubjects = new List<CourseSubject> { new CourseSubject { Subject = new Subject { SubjectName = "Geography"}}}, Name = "Geography" });

            _dataServiceMock.Setup(x => x.GetCoursesForUser(email, ProviderCode))
                .Returns(new List<Course> { new Course { CourseCode = CourseCode, Provider = provider, ProgramType = "SD", CourseSubjects = new List<CourseSubject> { new CourseSubject { Subject = new Subject { SubjectName = "History"}}}, Name = "History" }, new Course { CourseCode = CourseCode + "1", Provider = provider, ProgramType = "SD", CourseSubjects = new List<CourseSubject> { new CourseSubject { Subject = new Subject { SubjectName = "Geography"}}}, Name = "History" } } );

            _enrichmentServiceMock.Setup(x => x.GetProviderEnrichmentForPublish(ProviderCode, email))
                .Returns(new UcasProviderEnrichmentGetModel { EnrichmentModel = new ProviderEnrichmentModel() });

            var enrichmentModel = new CourseEnrichmentModel { FeeDetails = "It's gonna cost you", FeeInternational = (Decimal)123.5, FeeUkEu = (Decimal)234.5 };
            _enrichmentServiceMock.Setup(x => x.GetCourseEnrichmentForPublish(ProviderCode, CourseCode, email))
                .Returns(new UcasCourseEnrichmentGetModel { CourseCode = CourseCode, ProviderCode = ProviderCode, EnrichmentModel = enrichmentModel, Status = EnumStatus.Published });

            _enrichmentServiceMock.Setup(x => x.GetCourseEnrichmentForPublish(ProviderCode, CourseCode + "1", email))
                .Returns(new UcasCourseEnrichmentGetModel { CourseCode = CourseCode + "1", ProviderCode = ProviderCode, EnrichmentModel = enrichmentModel, Status = EnumStatus.Published });

            _httpMock.Setup(x => x.PutAsync(It.Is<Uri>(y => y.AbsoluteUri == $"{sncUrl}/courses"), It.IsAny<StringContent>())).ReturnsAsync(
                new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK
                }
            ).Verifiable();

            var result = _searchAndCompareService.SaveCourses(ProviderCode, email).Result;

            result.Should().BeTrue();
            _httpMock.VerifyAll();
        }

        [Test]
        public void PublishEnrichedCourseWithEmailDraftTest()
        {
            var email = "tester@example.com";
            Provider provider = new Provider
            {
                ProviderCode = ProviderCode,
                AccreditedCourses =
                    new List<Course> { new Course { CourseCode = CourseCode, ProgramType = "SD", Name = "History" } }
            };
            _dataServiceMock.Setup(x => x.GetProviderForUser(email, ProviderCode)).Returns(provider);
            _dataServiceMock.Setup(x => x.GetCourseForUser(email, ProviderCode, CourseCode))
                .Returns(new Course { CourseCode = CourseCode, Provider = provider, ProgramType = "SD", CourseSubjects = new List<CourseSubject> { new CourseSubject { Subject = new Subject { SubjectName = "History"}}}, Name = "History" });

            _enrichmentServiceMock.Setup(x => x.GetProviderEnrichmentForPublish(ProviderCode, email))
                .Returns(new UcasProviderEnrichmentGetModel { EnrichmentModel = new ProviderEnrichmentModel() });

            var enrichmentModel = new CourseEnrichmentModel { FeeDetails = "It's gonna cost you", FeeInternational = (Decimal)123.5, FeeUkEu = (Decimal)234.5 };
            _enrichmentServiceMock.Setup(x => x.GetCourseEnrichmentForPublish(ProviderCode, CourseCode, email))
                .Returns(new UcasCourseEnrichmentGetModel { CourseCode = CourseCode, ProviderCode = ProviderCode, EnrichmentModel = enrichmentModel, Status = EnumStatus.Draft });
            var result = _searchAndCompareService.SaveCourse(ProviderCode, CourseCode, email).Result;

            result.Should().BeFalse();
        }
        [Test]
        public void PublishEnrichedCoursesWithEmailDraftTest()
        {
            var email = "tester@example.com";
            var provider = new Provider
            {
                ProviderCode = ProviderCode,
                AccreditedCourses =
                    new List<Course> { new Course { CourseCode = CourseCode, ProgramType = "SD", Name = "History" }, new Course { CourseCode = CourseCode + "1", ProgramType = "SD", Name = "Geography" } }
            };

            _dataServiceMock.Setup(x => x.GetProviderForUser(email, ProviderCode)).Returns(provider);
            _dataServiceMock.Setup(x => x.GetCourseForUser(email, ProviderCode, CourseCode))
                .Returns(new Course { CourseCode = CourseCode, Provider = provider, ProgramType = "SD", CourseSubjects = new List<CourseSubject> { new CourseSubject { Subject = new Subject { SubjectName = "History"}}}, Name = "History" });

            _dataServiceMock.Setup(x => x.GetCourseForUser(email, ProviderCode, CourseCode + "1"))
                .Returns(new Course { CourseCode = CourseCode + "1", Provider = provider, ProgramType = "SD", CourseSubjects = new List<CourseSubject> { new CourseSubject { Subject = new Subject { SubjectName = "Geography"}}}, Name = "Geography" });

            _dataServiceMock.Setup(x => x.GetCoursesForUser(email, ProviderCode))
                .Returns(new List<Course> { new Course { CourseCode = CourseCode, Provider = provider, ProgramType = "SD", CourseSubjects = new List<CourseSubject> { new CourseSubject { Subject = new Subject { SubjectName = "History"}}}, Name = "History" }, new Course { CourseCode = CourseCode + "1", Provider = provider, ProgramType = "SD", CourseSubjects = new List<CourseSubject> { new CourseSubject { Subject = new Subject { SubjectName = "Geography"}}}, Name = "History" } } );

            _enrichmentServiceMock.Setup(x => x.GetProviderEnrichmentForPublish(ProviderCode, email))
                .Returns(new UcasProviderEnrichmentGetModel { EnrichmentModel = new ProviderEnrichmentModel() });

            var enrichmentModel = new CourseEnrichmentModel { FeeDetails = "It's gonna cost you", FeeInternational = (Decimal)123.5, FeeUkEu = (Decimal)234.5 };
            _enrichmentServiceMock.Setup(x => x.GetCourseEnrichmentForPublish(ProviderCode, CourseCode, email))
                .Returns(new UcasCourseEnrichmentGetModel { CourseCode = CourseCode, ProviderCode = ProviderCode, EnrichmentModel = enrichmentModel, Status = EnumStatus.Draft });

            _enrichmentServiceMock.Setup(x => x.GetCourseEnrichmentForPublish(ProviderCode, CourseCode + "1", email))
                .Returns(new UcasCourseEnrichmentGetModel { CourseCode = CourseCode + "1", ProviderCode = ProviderCode, EnrichmentModel = enrichmentModel, Status = EnumStatus.Published });

            var result = _searchAndCompareService.SaveCourses(ProviderCode, email).Result;

            result.Should().BeFalse();
        }

        [Test][Ignore("This test will fail atm. Revisit when we start publishing basic courses from here")]
        public void PublishBasicCourseWithEmailHappyPathTest()
        {
            var email = "tester@example.com";
            var provider = new Provider
            {
                ProviderCode = ProviderCode,
                AccreditedCourses =
                    new List<Course> { new Course { CourseCode = CourseCode, ProgramType = "SD", Name = "History" } }
            };
            _dataServiceMock.Setup(x => x.GetProviderForUser(email, ProviderCode)).Returns(provider);
            _dataServiceMock.Setup(x => x.GetCourseForUser(email, ProviderCode, CourseCode))
                .Returns(new Course { CourseCode = CourseCode, Provider = provider, ProgramType = "SD", CourseSubjects = new List<CourseSubject> { new CourseSubject { Subject = new Subject { SubjectName = "History"}}}, Name = "History" });

            _enrichmentServiceMock.Setup(x => x.GetProviderEnrichmentForPublish(ProviderCode, email))
                .Returns(new UcasProviderEnrichmentGetModel());

            _httpMock.Setup(x => x.PostAsync(It.Is<Uri>(y => y.AbsoluteUri == $"{sncUrl}/courses/{ProviderCode}/{CourseCode}"), It.IsAny<StringContent>())).ReturnsAsync(
                new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK
                }
            ).Verifiable();

            var result = _searchAndCompareService.SaveCourse(ProviderCode, CourseCode, email).Result;

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
        public void PublishCourseWithEmailInvalidParametersTest(string providerCode, string courseCode, string email)
        {
            var result = _searchAndCompareService.SaveCourse(providerCode, courseCode, email).Result;

            result.Should().BeFalse();
        }
        [TestCase("", "")]
        [TestCase("123", "")]
        [TestCase("", "email@qwe.com")]
        [TestCase("  ", "        ")]
        [TestCase("123  ", "        ")]
        [TestCase("  ", "email@qwe.com")]
        [TestCase(null, null)]
        [TestCase("123", null)]
        [TestCase(null, "email@qwe.com")]
        public void PublishCoursesWithEmailInvalidParametersTest(string providerCode, string email)
        {
            var result = _searchAndCompareService.SaveCourses(providerCode, email).Result;

            result.Should().BeFalse();
        }

        private static Course BuildCourse(Provider provider)
        {
            return new Course
            {
                CourseCode = CourseCode, Provider = provider, ProgramType = "SD",
                CourseSubjects = new List<CourseSubject>
                    {new CourseSubject {Subject = new Subject {SubjectName = "History"}}},
                Name = "History",
                CourseSites = new List<CourseSite>
                {
                    new CourseSite
                    {
                        Status = "R",
                        Publish = "Y",
                        Site = new Site(),
                    }
                }
            };
        }
    }
}
