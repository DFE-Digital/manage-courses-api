using System.Collections.Generic;
using FluentAssertions;
using GovUk.Education.ManageCourses.Domain.Models;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting.Model
{
    [TestFixture]
    public class CourseTests
    {
        [Test]
        public void IsPublished_NullSites()
        {
            var course = new Course
            {
                CourseSites = null,
            };
            course.IsPublished.Should().BeFalse();
        }

        [Test]
        public void IsPublished_NoSites()
        {
            var course = new Course
            {
                CourseSites = new List<CourseSite>(),
            };
            course.IsPublished.Should().BeFalse();
        }

        [Test]
        public void IsPublished_UnpublishedSites()
        {
            var course = new Course
            {
                CourseSites = new List<CourseSite>
                {
                    new CourseSite
                    {
                        Publish = "N",
                    }
                },
            };
            course.IsPublished.Should().BeFalse();
        }

        [Test]
        public void IsPublished_PublishedSites()
        {
            var course = new Course
            {
                CourseSites = new List<CourseSite>
                {
                    new CourseSite
                    {
                        Publish = "Y",
                    }
                },
            };
            course.IsPublished.Should().BeTrue();
        }
    }
}
