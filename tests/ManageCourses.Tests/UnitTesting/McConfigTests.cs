﻿using System;
using GovUk.Education.ManageCourses.Api;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    public class McConfigTests
    {
        [Test]
        public void MissingThrows()
        {
            var config = new Mock<IConfiguration>(); // none of the config mocked
            var mcConfig = new DatabaseConfig(config.Object);
            Assert.Throws<Exception>(mcConfig.Validate);
        }

        [Test]
        public void WhitespaceThrows()
        {
            var config = new Mock<IConfiguration>(); // none of the config mocked
            config.SetupGet(c => c[It.IsAny<string>()]).Returns("   ");
            var mcConfig = new DatabaseConfig(config.Object);
            Assert.Throws<Exception>(mcConfig.Validate);
        }

        [Test]
        public void AllAvailableDoesntThrow()
        {
            var config = new Mock<IConfiguration>();
            //config.SetupGet(c => c["MANAGE_COURSES_POSTGRESQL_SERVICE_HOST"]).Returns("hello");
            config.SetupGet(c => c[It.IsAny<string>()]).Returns("hello");
            var mcConfig = new DatabaseConfig(config.Object);
            mcConfig.Validate();
        }
    }
}
