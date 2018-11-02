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
            var course = GetBlankUcasCourse();
            course.StartMonth = "September";
            course.StartYear = "2019";

            var res = LoadCourse(course);

            res.StartDate.Should().Be(new DateTime(2019, 9, 1));
        }

        private static CourseLoader GetCourseLoader(List<Provider> providers = null)
        {
            var p = (providers ?? new List<Provider>()).ToDictionary(x => x.ProviderCode ?? "");
            return new CourseLoader(p, new Dictionary<string, Subject>(), new List<PgdeCourse>());
        }

        [Test]
        public void TolerateBlankStartDate()
        {
            var res = LoadCourse(GetBlankUcasCourse());

            res.StartDate.Should().BeNull();
        }

        [Test]
        public void CourseWithNoVacancies()
        {
            // arrange
            var blankUcasCourse = GetBlankUcasCourse(); // ucas course is the denormalised course+campus combinations
            const string noVacancies = "";
            blankUcasCourse.VacStatus = noVacancies;

            // act
            var courseRecords = new List<UcasCourse> { blankUcasCourse };
            var enrichmentMetadata = new List<UcasCourseEnrichmentGetModel>();
            var foo = new List<Subject>();
            var manageApiCourse = GetCourseLoader().LoadCourses(new Provider(), courseRecords, new List<UcasCourseSubject>(), new List<Site>{new Site()}).Single();

            // assert
            manageApiCourse.HasVacancies.Should().Be(false, "because there is only one course and it has no vacancies");
        }

        [Test]
        public void CourseWithVacancy()
        {
            // arrange
            var ucasCourseWithoutVacancy = GetBlankUcasCourse();
            var ucasCourseWithVacancy = GetBlankUcasCourse();
            const string fullTime = "F";
            ucasCourseWithVacancy.VacStatus = fullTime;

            // act
            var courseRecords = new List<UcasCourse> { ucasCourseWithoutVacancy, ucasCourseWithVacancy };
            var enrichmentMetadata = new List<UcasCourseEnrichmentGetModel>();
            var foo = new List<Subject>();
            var manageApiCourse = GetCourseLoader().LoadCourses(new Provider(), courseRecords, new List<UcasCourseSubject>(), new List<Site>{new Site()}).Single();

            // assert
            manageApiCourse.HasVacancies.Should().Be(true, "because there's one full time course");
        }

        [Test]
        public void MapsSchoolVacStatus()
        {
            // arrange
            var blankUcasCourse = GetBlankUcasCourse(); // ucas course is the denormalised course+campus combinations
            const string both = "B";
            blankUcasCourse.VacStatus = both;

            // act
            var courseRecords = new List<UcasCourse> { blankUcasCourse };
            var enrichmentMetadata = new List<UcasCourseEnrichmentGetModel>();
            var foo = new List<Subject>();
            var manageApiCourse = GetCourseLoader().LoadCourses(new Provider(), courseRecords, new List<UcasCourseSubject>(), new List<Site>{new Site()}).Single();

            // assert
            manageApiCourse.Sites.Should().HaveCount(1, "There's one campus");
            manageApiCourse.CourseSites.First().VacStatus.Should().Be(both);
        }

        [Test]
        public void RunningAndPublishedLocationsArePreferred()
        {
            var loc1 = GetBlankUcasCourse();
            loc1.Status = "S";
            loc1.AccreditingProvider = "WRONG_ACC";
            loc1.InstCode = "RIGHT_INST";

            var loc2 = GetBlankUcasCourse();
            loc2.Status = "R";
            loc2.AccreditingProvider = "RIGHT_ACC";
            loc2.InstCode = "RIGHT_INST";
            loc2.Publish = "Y";

            Provider provider = new Provider { ProviderCode = "RIGHT_INST" };
            var providers = new List<Provider>
            {
                new Provider { ProviderCode = "WRONG_ACC"},
                new Provider { ProviderCode = "RIGHT_ACC"},
                provider
            };

            var sites = new List<Site>
            {
                new Site { Provider = provider }
            };

            List<Subject> foo = new List<Subject>();
            var res = GetCourseLoader(providers).LoadCourses(provider, new List<UcasCourse> { loc1, loc2}, new List<UcasCourseSubject>(), sites).Single();

            res.AccreditingProvider.ProviderCode.Should().Be("RIGHT_ACC");
            res.Provider.ProviderCode.Should().Be("RIGHT_INST");
        }

        [Test]
        public void FullySuspendedCoursesWorkStill()
        {
            var loc1 = GetBlankUcasCourse();
            loc1.Status = "S";
            loc1.AccreditingProvider = "RIGHT_ACC";
            loc1.InstCode = "RIGHT_INST";

            var res = LoadCourse(loc1);

            res.AccreditingProvider.ProviderCode.Should().Be("RIGHT_ACC");
            res.Provider.ProviderCode.Should().Be("RIGHT_INST");
        }

        private static Course LoadCourse(UcasCourse course)
        {
            Provider provider = new Provider { ProviderCode = course.InstCode };
            var providers = new List<Provider> { provider };
            if (!string.IsNullOrWhiteSpace(course.AccreditingProvider))
            {
                providers.Add(new Provider { ProviderCode = course.AccreditingProvider });
            }
            List<Subject> foo = new List<Subject>();
            return GetCourseLoader(providers).LoadCourses(provider, new List<UcasCourse> { course }, new List<UcasCourseSubject>(), new List<Site> { new Site { Provider = provider, Code = course.CampusCode}}).Single();
        }

        private static UcasCourse GetBlankUcasCourse()
        {
            return new UcasCourse();
        }
    }
}