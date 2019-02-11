using System;
using GovUk.Education.ManageCourses.Api;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    [Category("Unit")]
    public class McConfigTests
    {
        [Test]
        public void MissingThrows()
        {
            var config = new Mock<IConfiguration>(); // none of the config mocked
            var mcConfig = new McConfig(config.Object);
            Assert.Throws<Exception>(mcConfig.Validate);
        }

        [Test]
        public void WhitespaceThrows()
        {
            var config = new Mock<IConfiguration>(); // none of the config mocked
            config.SetupGet(c => c[It.IsAny<string>()]).Returns("   ");
            var mcConfig = new McConfig(config.Object);
            Assert.Throws<Exception>(mcConfig.Validate);
        }

        [Test]
        public void AllAvailableDoesntThrow()
        {
            var config = new Mock<IConfiguration>();
            //config.SetupGet(c => c["MANAGE_COURSES_POSTGRESQL_SERVICE_HOST"]).Returns("hello");
            config.SetupGet(c => c[It.IsAny<string>()]).Returns("hello");
            var mcConfig = new McConfig(config.Object);
            mcConfig.Validate();
        }

        [Test]
        public void ThrowsForHalfEmailConfig()
        {
            var config = new Mock<IConfiguration>();
            config.SetupGet(c => c[It.IsNotIn("email:api_key")]).Returns("hello");
            var mcConfig = new McConfig(config.Object);
            Assert.Throws<Exception>(mcConfig.Validate);
        }

        [Test]
        public void ValidWithoutEmailConfig()
        {
            var config = new Mock<IConfiguration>();
            config.SetupGet(c => c[It.IsRegex("^(?!email:).*")]).Returns("hello");
            var mcConfig = new McConfig(config.Object);
            mcConfig.Validate();
        }
    }
}
