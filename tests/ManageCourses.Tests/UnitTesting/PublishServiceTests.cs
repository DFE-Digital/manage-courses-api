using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Services.Data;
using GovUk.Education.ManageCourses.Api.Services.Publish;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.SearchAndCompare.Domain.Client;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    public class PublishServiceTests
    {
        private Mock<IEnrichmentService> _enrichmentServiceMock;
        private Mock<IDataService> _dataServiceMock;
        private IPublishService _publishService;
        private Mock<ISearchAndCompareApi> _searchAndCompareApiMock;
        private const string institutionCode = "";
        private const string courseCode = "";

        [SetUp]
        public void SetUp()
        {
            _enrichmentServiceMock = new Mock<IEnrichmentService>();
            _searchAndCompareApiMock = new Mock<ISearchAndCompareApi>();
            _dataServiceMock = new Mock<IDataService>();

            _publishService = new PublishService(_searchAndCompareApiMock.Object, new CourseMapper(), _dataServiceMock.Object, _enrichmentServiceMock.Object);
        }

        [Test]
        public void PublishCourseHappyPathTest()
        {
            _dataServiceMock.Setup(x => x.GetUcasInstitution(institutionCode)).Returns(new UcasInstitution
            {
                AccreditedUcasCourses =
                    new List<UcasCourse> { new UcasCourse { InstCode = institutionCode, CrseCode = courseCode } }
            });
            _dataServiceMock.Setup(x => x.GetCourse(institutionCode, courseCode))
                .Returns(new Course { CourseCode = courseCode, InstCode = institutionCode });

            _enrichmentServiceMock.Setup(x => x.GetInstitutionEnrichment(institutionCode))
                .Returns(new UcasInstitutionEnrichmentGetModel());
            _enrichmentServiceMock.Setup(x => x.GetCourseEnrichment(institutionCode, courseCode))
                .Returns(new UcasCourseEnrichmentGetModel { CourseCode = courseCode, InstCode = institutionCode });
            _searchAndCompareApiMock.Setup(x => x.SaveCoursesAsync(It.IsAny<List<SearchAndCompare.Domain.Models.Course>>())).ReturnsAsync(true);

            var result = _publishService.PublishCourse(institutionCode, courseCode).Result;

            result.Should().BeTrue();
        }
        [Test]
        public void PublishCourseWithEmailHappyPathTest()
        {
            var email = "tester@example.com";
            _dataServiceMock.Setup(x => x.GetUcasInstitutionForUser(email, institutionCode)).Returns(new UcasInstitution
            {
                AccreditedUcasCourses =
                    new List<UcasCourse> { new UcasCourse { InstCode = institutionCode, CrseCode = courseCode } }
            });
            _dataServiceMock.Setup(x => x.GetCourse(email, institutionCode, courseCode))
                .Returns(new Course { CourseCode = courseCode, InstCode = institutionCode });

            _enrichmentServiceMock.Setup(x => x.GetInstitutionEnrichment(institutionCode, email))
                .Returns(new UcasInstitutionEnrichmentGetModel());
            _enrichmentServiceMock.Setup(x => x.GetCourseEnrichment(institutionCode, courseCode, email))
                .Returns(new UcasCourseEnrichmentGetModel { CourseCode = courseCode, InstCode = institutionCode });
            _searchAndCompareApiMock.Setup(x => x.SaveCoursesAsync(It.IsAny<List<SearchAndCompare.Domain.Models.Course>>())).ReturnsAsync(true);

            var result = _publishService.PublishCourse(institutionCode, courseCode, email).Result;

            result.Should().BeTrue();
        }
    }
}
