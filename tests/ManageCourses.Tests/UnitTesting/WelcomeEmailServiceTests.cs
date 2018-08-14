using System.Collections.Generic;
using GovUk.Education.ManageCourses.Api.Services.Email;
using GovUk.Education.ManageCourses.Api.Services.Email.Config;
using GovUk.Education.ManageCourses.Api.Services.Email.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    public class WelcomeEmailServiceTests
    {
        private IWelcomeEmailService _service;
        private Mock<INotificationClientWrapper> _mockNotificationClientWrapper;
        private Mock<IWelcomeTemplateEmailConfig> _mockWelcomeTemplateEmailConfig;

        private readonly string _templateId = "mockWelcomeTemplateEmailConfig templateId";
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mockNotificationClientWrapper = new Mock<INotificationClientWrapper>();
            _mockWelcomeTemplateEmailConfig = new Mock<IWelcomeTemplateEmailConfig>();


            _mockWelcomeTemplateEmailConfig.Setup(x => x.TemplateId).Returns(_templateId);

            _service = new WelcomeEmailService(_mockNotificationClientWrapper.Object, _mockWelcomeTemplateEmailConfig.Object);
        }

        [Test]
        public void Send()
        {
            var user = new McUser()
            {
                Email = "actual email address",
                FirstName = "FirstName  there are spaces at the end             "
            };

            var personalisation = new Dictionary<string, dynamic>() {
                {"first_name", user.FirstName.Trim() } };

            var model = new WelcomeEmailModel(user);
            _service.Send(model);

            _mockNotificationClientWrapper.Verify(x => x.SendEmail(user.Email, _templateId, personalisation, null, null), Times.Once);
        }
    }
}
