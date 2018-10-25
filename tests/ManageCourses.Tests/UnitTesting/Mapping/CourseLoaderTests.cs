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
using GovUk.Education.ManageCourses.UcasCourseImporter.Mapping;
using GovUk.Education.ManageCourses.Xls.Domain;

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
            var foo = new List<Subject>();
            var manageApiCourse = courseLoader.LoadCourses(courseRecords, new List<UcasCourseSubject>(), new List<UcasSubject>(), new List<PgdeCourse>(), ref foo, new List<Site>{new Site()}, new List<Institution>()).Single();

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
            var foo = new List<Subject>();
            var manageApiCourse = courseLoader.LoadCourses(courseRecords, new List<UcasCourseSubject>(), new List<UcasSubject>(), new List<PgdeCourse>(), ref foo, new List<Site>{new Site()}, new List<Institution>()).Single();

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
            var foo = new List<Subject>();
            var manageApiCourse = courseLoader.LoadCourses(courseRecords, new List<UcasCourseSubject>(), new List<UcasSubject>(), new List<PgdeCourse>(), ref foo, new List<Site>{new Site()}, new List<Institution>()).Single();

            // assert
            manageApiCourse.Sites.Should().HaveCount(1, "There's one campus");
            manageApiCourse.CourseSites.First().VacStatus.Should().Be(both);
        }

        [Test]
        public void RunningAndPublishedLocationsArePreferred()
        {
            var sut = new CourseLoader();
            var loc1 = GetBlankUcasCourse();
            loc1.Status = "S";
            loc1.AccreditingProvider = "WRONG_ACC";
            loc1.InstCode = "RIGHT_INST";

            var loc2 = GetBlankUcasCourse();
            loc2.Status = "R";
            loc2.AccreditingProvider = "RIGHT_ACC";
            loc2.InstCode = "RIGHT_INST";
            loc2.Publish = "Y";

            Institution institution = new Institution { InstCode = "RIGHT_INST" };
            var institutions = new List<Institution>
            {
                new Institution { InstCode = "WRONG_ACC"},
                new Institution { InstCode = "RIGHT_ACC"},
                institution
            };

            var sites = new List<Site>
            {
                new Site { Institution = institution }
            };

            List<Subject> foo = new List<Subject>();
            var res = sut.LoadCourses(new List<UcasCourse> { loc1, loc2}, new List<UcasCourseSubject>(), new List<UcasSubject>(), new List<PgdeCourse>(), ref foo, sites, institutions).Single();

            res.AccreditingInstitution.InstCode.Should().Be("RIGHT_ACC");
            res.Institution.InstCode.Should().Be("RIGHT_INST");
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

            res.AccreditingInstitution.InstCode.Should().Be("RIGHT_ACC");
            res.Institution.InstCode.Should().Be("RIGHT_INST");
        }

        private static Course LoadCourse(CourseLoader sut, UcasCourse course)
        {
            Institution institution = new Institution { InstCode = course.InstCode };
            var providers = new List<Institution> { institution };
            if (!string.IsNullOrWhiteSpace(course.AccreditingProvider))
            {
                providers.Add(new Institution { InstCode = course.AccreditingProvider });
            }
            List<Subject> foo = new List<Subject>();
            return sut.LoadCourses(new List<UcasCourse> { course }, new List<UcasCourseSubject>(), new List<UcasSubject>(), new List<PgdeCourse>(), ref foo,  new List<Site> { new Site { Institution = institution, Code = course.CampusCode}}, providers).Single();
        }

        private static UcasCourse GetBlankUcasCourse()
        {
            return new UcasCourse();
        }
    }
}