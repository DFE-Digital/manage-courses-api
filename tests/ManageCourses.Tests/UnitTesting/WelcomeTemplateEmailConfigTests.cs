using NUnit.Framework;

using Moq;

using Microsoft.Extensions.Configuration;

using GovUk.Education.ManageCourses.Api.Services.Email.Config;
using GovUk.Education.ManageCourses.Api.Services.Email.Model;

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

            var subject = new WelcomeTemplateEmailConfig(configMock.Object);

            Assert.AreEqual(configId, subject.ConfigId);
            Assert.AreEqual(templateId, subject.TemplateId);
            Assert.AreEqual(typeof(WelcomeEmailModel), subject.Type);
        }
    }
}
