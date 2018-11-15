using System.Collections.Generic;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Controllers;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.DbIntegration.Controllers
{
    [TestFixture]
    [Category("Integration")]
    [Category("Integration_DB")]
    public class DevOpsControllerTests : DbIntegrationTestBase
    {
        private DevOpsController _devOpsController;

        protected override void Setup()
        {
            _devOpsController = new DevOpsController(new Mock<ILogger<DevOpsController>>().Object, Context);
        }

        [Test]
        public void PingShouldFailWithNoCourses()
        {
            var result = _devOpsController.Ping();
            result.Should().NotBeNull();
            const string because = "we haven't put any courses in the db yet";
            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError, because);
            result.Value.Should().BeOfType<string>();
            var stringResult = result.Value as string;
            stringResult.Should().Contain("Not enough courses", because);
        }

        [Test]
        public void TestPing()
        {
            // arrange
            var courses = new List<Course>();
            var provider = new Provider { ProviderName = "billy goat school" };
            for (var i = 0; i < 20; i++)
            {
                courses.Add(new Course { Name = "course" + i, Provider = provider });
            }
            Context.Courses.AddRange(courses);
            Context.Save();

            // act
            var result = _devOpsController.Ping();

            // assert
            result.Should().NotBeNull();
            const string because = "There are enough courses in the db";
            result.StatusCode.Should().Be(StatusCodes.Status200OK, because);
            result.Value.Should().BeOfType<string>();
            var stringResult = result.Value as string;
            stringResult.Should().Contain("courses", because);

            Context.Courses.RemoveRange(courses);
            Context.Save();
        }
    }
}
