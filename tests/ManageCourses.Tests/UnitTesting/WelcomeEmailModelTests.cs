using System.Collections.Generic;
using NUnit.Framework;

using Moq;

using GovUk.Education.ManageCourses.Api.Services.Email.Model;

using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    public class WelcomeEmailModelTests
    {
        [Test]
        public void Constructor_Test()
        {
            var user = new McUser()
            {
                Email = "actual email address",
                FirstName = "FirstName  there are spaces at the end             "
            };

            var personalisation = new Dictionary<string, dynamic>() {
                {"first_name", user.FirstName.Trim() } };

            var model = new WelcomeEmailModel(user);

            Assert.AreEqual(user.Email, model.EmailAddress);
            CollectionAssert.AreEqual(personalisation, model.Personalisation);
        }
    }
}
