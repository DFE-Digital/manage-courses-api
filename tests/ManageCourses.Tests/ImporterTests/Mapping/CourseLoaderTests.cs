using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.UcasCourseImporter.Mapping;
using GovUk.Education.ManageCourses.Xls.Domain;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GovUk.Education.ManageCourses.UcasCourseImporter.Tests
{
    [TestFixture]
    public class CourseLoaderTests
    {
        [Test]
        public void LoadCourseTest()
        {
            const string providerCode = "PR1";
            const string campusCode = "C";
            Provider provider = new Provider { ProviderCode = providerCode };

            var providers = new Dictionary<string, Provider>
            {
                [providerCode] = provider
            };
            var subjects = new Dictionary<string, Subject>();
            var pgdeCourses = new List<PgdeCourse>();
            var courseLoader = new CourseLoader(providers, subjects, pgdeCourses);

            var courseRecords = new List<UcasCourse>
            {
                new UcasCourse{ CrseCode = "CRS1", CampusCode = campusCode, InstCode = providerCode },
                new UcasCourse{ CrseCode = "CRS2", CampusCode = campusCode, InstCode = providerCode, Modular = "" },
                new UcasCourse{ CrseCode = "CRS3", CampusCode = campusCode, InstCode = providerCode, Modular = "M" }
            };
            var courseSubjects = new List<UcasCourseSubject>();
            var allSites = new List<Site>{
                new Site{ Provider = provider, Code = campusCode }
            };
            var returnedCourses = courseLoader.LoadCourses(provider, courseRecords, courseSubjects, allSites);

            returnedCourses.Should().HaveCount(3, "3 courses were passed in");
            returnedCourses[0].Modular.Should().Be(null, "The first course doesn't specify Modular");
            returnedCourses[1].Modular.Should().Be("", "The second course has an empty string Modular");
            returnedCourses[2].Modular.Should().Be("M", "The third course has a defined Modular of 'M'");
        }

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
