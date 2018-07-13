using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using GovUk.Education.ManageCourses.Tests.Integration.DatabaseAccess;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Moq;
namespace GovUk.Education.ManageCourses.Tests.Integration
{
    [TestFixture]
    [Category("Integration")]
    [Explicit]
    [Ignore("For debugging only")]
    public class WelcomeEmailServiceTests 
    {
        public IWelcomeEmailService Subject = null;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var apiKey = "";
            var templateId = "";
            Subject = new WelcomeEmailService(apiKey, templateId);
        }
        
        [Test]
        [Ignore("For debugging only")]
        public void Send()
        {
            var user = new McUser()
            {
                Email = "actual email address",
                FirstName = "FirstName"
            };
            Subject.Send(user);
        }
    }
}
