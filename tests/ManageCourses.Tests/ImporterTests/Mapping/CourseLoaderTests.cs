using NUnit.Framework;
using FluentAssertions;
using GovUk.Education.ManageCourses.UcasCourseImporter.Mapping;
using System.Collections.Generic;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Xls.Domain;

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
    }
}
