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

        [Test]
        public void CourseWithNoVacancies()
        {
            // arrange
            var courseLoader = new CourseLoader();
            var blankUcasCourse = GetBlankUcasCourse(); // ucas course is the denormalised course+campus combinations
            const string noVacancies = "";
            blankUcasCourse.VacStatus = noVacancies;

            // act
            var courseRecords = new List<UcasCourse> { blankUcasCourse };
            var enrichmentMetadata = new List<UcasCourseEnrichmentGetModel>();
            const bool pgde = false;
            var manageApiCourse = courseLoader.LoadCourse(courseRecords, enrichmentMetadata, pgde);

            // assert
            manageApiCourse.HasVacancies.Should().Be(false, "because there is only one course and it has no vacancies");
        }

        [Test]
        public void CourseWithVacancy()
        {
            // arrange
            var courseLoader = new CourseLoader();
            var ucasCourseWithoutVacancy = GetBlankUcasCourse();
            var ucasCourseWithVacancy = GetBlankUcasCourse();
            const string fullTime = "F";
            ucasCourseWithVacancy.VacStatus = fullTime;

            // act
            var courseRecords = new List<UcasCourse> { ucasCourseWithoutVacancy, ucasCourseWithVacancy };
            var enrichmentMetadata = new List<UcasCourseEnrichmentGetModel>();
            const bool pgde = false;
            var manageApiCourse = courseLoader.LoadCourse(courseRecords, enrichmentMetadata, pgde);

            // assert
            manageApiCourse.HasVacancies.Should().Be(true, "because there's one full time course");
        }

        [Test]
        public void MapsSchoolVacStatus()
        {
            // arrange
            var courseLoader = new CourseLoader();
            var blankUcasCourse = GetBlankUcasCourse(); // ucas course is the denormalised course+campus combinations
            const string both = "B";
            blankUcasCourse.VacStatus = both;

            // act
            var courseRecords = new List<UcasCourse> { blankUcasCourse };
            var enrichmentMetadata = new List<UcasCourseEnrichmentGetModel>();
            const bool pgde = false;
            var manageApiCourse = courseLoader.LoadCourse(courseRecords, enrichmentMetadata, pgde);

            // assert
            manageApiCourse.Schools.Should().HaveCount(1, "There's one campus");
            manageApiCourse.Schools.First().VacStatus.Should().Be(both);
        }

        [Test]
        public void RunningLocationsArePreferred()
        {
            var sut = new CourseLoader();
            var loc1 = GetBlankUcasCourse();
            loc1.Status = "S";
            loc1.AccreditingProvider = "WRONG_ACC";
            loc1.InstCode = "WRONG_INST";

            var loc2 = GetBlankUcasCourse();
            loc2.Status = "R";
            loc2.AccreditingProvider = "RIGHT_ACC";
            loc2.InstCode = "RIGHT_INST";

            var res = sut.LoadCourse(new List<UcasCourse> {loc1, loc2}, new List<UcasCourseEnrichmentGetModel>(), false);

            res.AccreditingProviderId.Should().Be("RIGHT_ACC");
            res.InstCode.Should().Be("RIGHT_INST");
        }

        [Test]
        public void FullySuspendedCoursesWorkStill()
        {            
            var sut = new CourseLoader();
            var loc1 = GetBlankUcasCourse();
            loc1.Status = "S";
            loc1.AccreditingProvider = "RIGHT_ACC";
            loc1.InstCode = "RIGHT_INST";

            var res = LoadCourse(sut, loc1);

            res.AccreditingProviderId.Should().Be("RIGHT_ACC");
            res.InstCode.Should().Be("RIGHT_INST");
        }

        private static Course LoadCourse(CourseLoader sut, UcasCourse course)
        {
            return sut.LoadCourse(new List<UcasCourse> { course }, new List<UcasCourseEnrichmentGetModel>(), false);
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