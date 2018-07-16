using System.Collections.Generic;
using NUnit.Framework;
using GovUk.Education.ManageCourses.Api.Services;

using GovUk.Education.ManageCourses.Domain.Models;
using Moq;
using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.Tests.Integration
{
    [TestFixture]
    public class WelcomeEmailServiceTests
    {
        private IWelcomeEmailService Subject = null;
        private Mock<INotificationClientWrapper> mock = null;
        private string templateId = "templateId";
        private Mock<IConfiguration> configMock = null;
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            mock = new Mock<INotificationClientWrapper>();
            configMock = new Mock<IConfiguration>();
            configMock.Setup(x => x["email:welcome_template_id"]).Returns(templateId);

            Subject = new WelcomeEmailService(mock.Object, configMock.Object);
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

            Subject.Send(user);

            mock.Verify(x => x.SendEmail(user.Email, templateId, personalisation, null, null), Times.Once);
            configMock.Verify(x => x["email:welcome_template_id"], Times.Once);
        }
    }
}
