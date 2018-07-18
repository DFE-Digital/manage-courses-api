using System.Collections.Generic;
using NUnit.Framework;
using GovUk.Education.ManageCourses.Api.Services;

using GovUk.Education.ManageCourses.Domain.Models;
using Moq;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    public class TemplateEmailServiceFactoryTests
    {
        private ITemplateEmailServiceFactory Subject = null;
        private Mock<INotificationClientWrapper> mock = null;
        private Mock<IConfiguration> mockConfig = null;
        private Mock<ILogger<TemplateEmailServiceFactory>> mockLogger = null;
        private string templateKey = "templateKey";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            mock = new Mock<INotificationClientWrapper>();
            mockConfig = new Mock<IConfiguration>();
            mockLogger = new Mock<ILogger<TemplateEmailServiceFactory>>();

            Subject = new TemplateEmailServiceFactory(mock.Object, mockConfig.Object, mockLogger.Object);
        }

        [Test]
        public void Build()
        {
            var templateId = "templateId";
            mockConfig.Setup(x => x[templateKey]).Returns(templateId);

            var emailService = Subject.Build(templateKey);

            Assert.IsNotNull(emailService);
            mockConfig.Verify(x => x[templateKey], Times.Once);
        }
    }
}
