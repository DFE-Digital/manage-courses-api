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
    }
}
