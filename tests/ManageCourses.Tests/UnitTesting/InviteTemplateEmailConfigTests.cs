using GovUk.Education.ManageCourses.Api.Services.Email.Config;
using GovUk.Education.ManageCourses.Api.Services.Email.Model;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    public class InviteTemplateEmailConfigTests
    {
        private readonly string _configId = "email:invite_template_id";
        private readonly string _templateId = "invite_template_id";

        [Test]
        public void Constructor_Test()
        {
            var configMock = new Mock<IConfiguration>();

            configMock.Setup(x => x[_configId]).Returns(_templateId);

            var emailConfig = new InviteTemplateEmailConfig(configMock.Object);

            Assert.AreEqual(_configId, emailConfig.ConfigId);
            Assert.AreEqual(_templateId, emailConfig.TemplateId);
            Assert.AreEqual(typeof(InviteEmailModel), emailConfig.Type);
        }
    }
}
