using System.Collections.Generic;
using NUnit.Framework;

using Moq;

using GovUk.Education.ManageCourses.Api.Services.Email;
using GovUk.Education.ManageCourses.Api.Services.Email.Config;
using GovUk.Education.ManageCourses.Api.Services.Email.Model;

using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    public class InviteEmailServiceTests
    {
        private IInviteEmailService Service = null;
        private Mock<INotificationClientWrapper> mockNotificationClientWrapper = null;
        private Mock<IInviteTemplateEmailConfig> mockInviteTemplateEmailConfig = null;

        private string templateId = "mockInviteTemplateEmailConfig templateId";
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            mockNotificationClientWrapper = new Mock<INotificationClientWrapper>();
            mockInviteTemplateEmailConfig = new Mock<IInviteTemplateEmailConfig>();

            
            mockInviteTemplateEmailConfig.Setup(x => x.TemplateId).Returns(templateId);

            Service = new InviteEmailService(mockNotificationClientWrapper.Object, mockInviteTemplateEmailConfig.Object);
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
            Service.Send(model);

            mockNotificationClientWrapper.Verify(x => x.SendEmail(user.Email, templateId, personalisation, null, null), Times.Once);
        }
    }
}
