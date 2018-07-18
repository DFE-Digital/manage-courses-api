using System.Collections.Generic;
using NUnit.Framework;
using GovUk.Education.ManageCourses.Api.Services;

using GovUk.Education.ManageCourses.Domain.Models;
using Moq;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    public class WelcomeEmailServiceTests
    {
        private IWelcomeEmailService Subject = null;
        private Mock<ITemplateEmailService> mock = null;
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            mock = new Mock<ITemplateEmailService>();
            Subject = new WelcomeEmailService(mock.Object);
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

            mock.Verify(x => x.Send(user.Email, personalisation), Times.Once);
        }
    }
}
