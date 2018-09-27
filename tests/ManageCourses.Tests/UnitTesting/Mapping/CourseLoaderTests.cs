using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.SearchAndCompare.Domain.Models.Enums;
using GovUk.Education.ManageCourses.Domain.Models;
using NUnit.Framework;
using GovUk.Education.ManageCourses.Api.Model;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting.Mapping
{
    [TestFixture]
    public class CourseLoaderTests
    {
        [Test]
        public void ParseStartDate()
        {
            var sut = new CourseLoader();

            var course = GetBlankUcasCourse();
            course.StartMonth = "September";
            course.StartYear = "2019";

            var res = LoadCourse(sut, course);

            res.StartDate.Should().Be(new DateTime(2019, 9, 1));
        }

        [Test]
        public void TolerateBlankStartDate()
        {
            var sut = new CourseLoader();

            var res = LoadCourse(sut, GetBlankUcasCourse());

            res.StartDate.Should().BeNull();
        }

        private static Course LoadCourse(CourseLoader sut, UcasCourse course)
        {
            return sut.LoadCourse(new List<UcasCourse> { course }, new List<UcasCourseEnrichmentGetModel>());
        }

        private static UcasCourse GetBlankUcasCourse()
        {
            return new UcasCourse
            {
                CourseCode = new CourseCode { UcasCourseSubjects = new Collection<UcasCourseSubject>() },
                UcasCampus = new UcasCampus()
            };
        }
    }
}