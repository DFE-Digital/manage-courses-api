using GovUk.Education.ManageCourses.Api.Services.Email.Config;
using GovUk.Education.ManageCourses.Api.Services.Email.Model;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    public class WelcomeTemplateEmailConfigTests
    {
        private string configId = "email:welcome_template_id";
        private string templateId = "welcome_template_id";

        [Test]
        public void Constructor_Test()
        {
            var configMock = new Mock<IConfiguration>();

            configMock.Setup(x => x[configId]).Returns(templateId);

            var emailConfig = new WelcomeTemplateEmailConfig(configMock.Object);

            Assert.AreEqual(configId, emailConfig.ConfigId);
            Assert.AreEqual(templateId, emailConfig.TemplateId);
            Assert.AreEqual(typeof(WelcomeEmailModel), emailConfig.Type);
        }
    }
}
