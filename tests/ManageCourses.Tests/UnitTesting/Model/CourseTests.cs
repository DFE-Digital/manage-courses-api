using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GovUk.Education.ManageCourses.Domain.Models;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting.Model
{
    [TestFixture]
    public class CourseTests
    {
        [Test]
        public void PublishableSites_NullSites()
        {
            var course = new Course
            {
                CourseSites = null,
            };
            course.PublishableSites.Should().HaveCount(0);
        }

        [Test]
        public void PublishableSites_NoSites()
        {
            var course = new Course
            {
                CourseSites = new List<CourseSite>(),
            };
            course.PublishableSites.Should().HaveCount(0);
        }

        [Test]
        // uppercase
        [TestCase("N", "N", false)]
        [TestCase("R", "N", false)]
        [TestCase("S", "N", false)]
        [TestCase("D", "N", false)]
        [TestCase("N", "Y", false)]
        [TestCase("R", "Y", true)]
        [TestCase("S", "Y", false)]
        [TestCase("D", "Y", false)]
        // lowercase
        [TestCase("n", "n", false)]
        [TestCase("r", "n", false)]
        [TestCase("s", "n", false)]
        [TestCase("d", "n", false)]
        [TestCase("n", "y", false)]
        [TestCase("r", "y", true)]
        [TestCase("s", "y", false)]
        [TestCase("d", "y", false)]
        public void PublishableSites_AllVariations(string status, string publish, bool publishable)
        {
            var course = new Course
            {
                CourseSites = new List<CourseSite>
                {
                    new CourseSite
                    {
                        Status = status,
                        Publish = publish,
                    }
                },
            };
            course.PublishableSites.Should().HaveCount(publishable ? 1 : 0);
        }

        [Test]
        // single site
        [TestCase("R", "Y", "B", null, null, null, true)]
        [TestCase("R", "Y", "F", null, null, null, true)]
        [TestCase("R", "Y", "P", null, null, null, true)]
        [TestCase("R", "Y", "",  null, null, null, false)]
        // other single-site variations do not show on find so vacancy information is intentionally left undefined
        // multi-site with non-publishable second sites:
        // https://trello.com/c/ddBrkrtk/1660-failing-to-sync-closed-courses-when-vacancies-turned-off
        [TestCase("R", "Y", "F", "R", "N", "B",  true)]
        [TestCase("R", "Y", "F", "R", "N", "",   true)]
        [TestCase("R", "Y", "F", "S", "Y", "B",  true)]
        [TestCase("R", "Y", "F", "S", "Y", "",   true)]
        [TestCase("R", "N", "",  "R", "N", "B",  false)]
        [TestCase("R", "N", "",  "R", "N", "",   false)]
        [TestCase("R", "N", "",  "S", "Y", "B",  false)]
        [TestCase("R", "N", "",  "S", "Y", "",   false)]
        public void HasVacancies_AllVariations(
            string site1status, string site1publish, string site1vacstatus,
            string site2status, string site2publish, string site2vacstatus,
            bool expectedCourseVacancies)
        {
            var course = new Course
            {
                CourseSites = new List<CourseSite>
                {
                    new CourseSite {Status = site1status, Publish = site1publish, VacStatus = site1vacstatus},
                },
            };
            if (site2status != null)
            {
                course.CourseSites.Add(
                    new CourseSite {Status = site2status, Publish = site2publish, VacStatus = site2vacstatus}
                );
            }
            course.HasVacancies.Should().Be(expectedCourseVacancies);
        }

    }
}
