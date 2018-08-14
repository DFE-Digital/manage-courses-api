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
    public class InviteEmailServiceTests
    {
        private IInviteEmailService _service;
        private Mock<INotificationClientWrapper> _mockNotificationClientWrapper;
        private Mock<IInviteTemplateEmailConfig> _mockInviteTemplateEmailConfig;
        private readonly string _templateId = "mockInviteTemplateEmailConfig templateId";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mockNotificationClientWrapper = new Mock<INotificationClientWrapper>();
            _mockInviteTemplateEmailConfig = new Mock<IInviteTemplateEmailConfig>();
            _mockInviteTemplateEmailConfig.Setup(x => x.TemplateId).Returns(_templateId);

            _service = new InviteEmailService(_mockNotificationClientWrapper.Object, _mockInviteTemplateEmailConfig.Object);
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

            var model = new InviteEmailModel(user);
            _service.Send(model);

            _mockNotificationClientWrapper.Verify(x => x.SendEmail(user.Email, _templateId, personalisation, null, null), Times.Once);
        }
    }
}
