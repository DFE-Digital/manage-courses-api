using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Moq;

using GovUk.Education.ManageCourses.Tests.Integration.DatabaseAccess;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Services.Email;
using GovUk.Education.ManageCourses.Api.Services.Email.Model;

using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Tests.Integration
{
    [TestFixture]
    [Category("Integration")]
    [Category("Integration_DB")]
    [Explicit]
    public class UserLogServiceTests : ManageCoursesDbContextIntegrationBase
    {
        public IUserLogService Subject = null;

        public Mock<IWelcomeEmailService> mockWelcomeEmailService = null;
        private const string TestUserEmail_1 = "email_1@test-manage-courses.gov.uk";
        private const string TestUserEmail_2 = "email_2@test-manage-courses.gov.uk";

        public McUser GetUser(string email, bool fromDb = true)
        {
            if (fromDb)
            {
                return context.McUsers.Single(x => x.Email == email);
            }
            else
            {
                return new McUser
                {
                    FirstName = "FirstName_" + email,
                    LastName = "LastName_" + email,
                    Email = email,
                };
            }
        }

        [SetUp]
        public void Setup()
        {
            context.AddMcUser(GetUser(TestUserEmail_1, false));
            context.AddMcUser(GetUser(TestUserEmail_2, false));

            context.SaveChanges();
            mockWelcomeEmailService = new Mock<IWelcomeEmailService>();

            Subject = new UserLogService(this.context, mockWelcomeEmailService.Object);
        }

        [TearDown]
        override public void TearDown()
        {

            foreach (var item in context.McUsers)
            {
                var x = ((DbContext)context).Entry(item);
                this.entitiesToCleanUp.Add(x);
            }

            foreach (var item in context.UserLogs)
            {
                var x = ((DbContext)context).Entry(item);
                this.entitiesToCleanUp.Add(x);
            }
            base.TearDown();
        }

        [Test]
        public void UpdateUserLog()
        {
            var signInUserId = "signInUserId";
            var firstLoginDateUtc = DateTime.UtcNow.AddDays(-10);

            var existing = context.UserLogs.Add(new UserLog { SignInUserId = "signInUserId", FirstLoginDateUtc = firstLoginDateUtc, LastLoginDateUtc = firstLoginDateUtc, User = context.McUsers.First() });

            context.SaveChanges();

            var result = Subject.CreateOrUpdateUserLog(signInUserId, GetUser(TestUserEmail_1));

            Assert.IsTrue(result);

            Assert.AreEqual(1, context.UserLogs.Count());

            var saved = context.UserLogs.First();
            Assert.IsNotNull(saved.User);
            Assert.AreEqual(firstLoginDateUtc, saved.FirstLoginDateUtc);
            Assert.AreNotEqual(firstLoginDateUtc, saved.LastLoginDateUtc);
            Assert.IsNull(saved.WelcomeEmailDateUtc);

            mockWelcomeEmailService.Verify(x => x.Send(It.IsAny<WelcomeEmailModel>()), Times.Never);
        }

        [Test]
        public void CreateUserLog()
        {
            var signInUserId = "signInUserId";
            var result = Subject.CreateOrUpdateUserLog(signInUserId, GetUser(TestUserEmail_1));

            Assert.IsTrue(result);

            Assert.AreEqual(1, context.UserLogs.Count());

            var result2 = Subject.CreateOrUpdateUserLog(signInUserId + 2, GetUser(TestUserEmail_2));

            Assert.IsTrue(result2);

            Assert.AreEqual(2, context.UserLogs.Count());

            foreach (var userLog in context.UserLogs)
            {
                Assert.IsNotNull(userLog.WelcomeEmailDateUtc);
            }

            mockWelcomeEmailService.Verify(x => x.Send(It.IsAny<WelcomeEmailModel>()), Times.Exactly(2));

            var welcomeModel1 = new WelcomeEmailModel(GetUser(TestUserEmail_1, true));
            mockWelcomeEmailService.Verify(x => x.Send(
                It.Is<WelcomeEmailModel>(model =>
                    (model.EmailAddress == welcomeModel1.EmailAddress)
                    )
                ), Times.Once);

            var welcomeModel2 = new WelcomeEmailModel(GetUser(TestUserEmail_2, true));
            mockWelcomeEmailService.Verify(x => x.Send(It.Is<WelcomeEmailModel>(model =>
                    (model.EmailAddress == welcomeModel2.EmailAddress)
                )), Times.Once);
        }


        [Test]
        public void CreateOrUpdateUserLog()
        {
            var signInUserId = "signInUserId";
            var result = Subject.CreateOrUpdateUserLog(signInUserId, GetUser(TestUserEmail_1));

            Assert.IsTrue(result);

            Assert.AreEqual(1, context.UserLogs.Count());
            Assert.IsNotNull(context.UserLogs.First().WelcomeEmailDateUtc);

            var result2 = Subject.CreateOrUpdateUserLog(signInUserId, GetUser(TestUserEmail_2));

            Assert.IsTrue(result2);

            Assert.AreEqual(1, context.UserLogs.Count());

            var welcomeModel1 = new WelcomeEmailModel(GetUser(TestUserEmail_1, true));
            mockWelcomeEmailService.Verify(x => x.Send(
                It.Is<WelcomeEmailModel>(model =>
                    (model.EmailAddress == welcomeModel1.EmailAddress)                    
                    )
                ), Times.Once);

            var welcomeModel2 = new WelcomeEmailModel(GetUser(TestUserEmail_2, true));
            mockWelcomeEmailService.Verify(x => x.Send(It.Is<WelcomeEmailModel>(model =>
                    (model.EmailAddress == welcomeModel2.EmailAddress)
                )), Times.Never);

            mockWelcomeEmailService.Verify(x => x.Send(It.IsAny<WelcomeEmailModel>()), Times.Once);
        }

        [Test]
        public void CreateOrUpdateUserLog_Deleted_MCuser()
        {
            Assert.AreEqual(0, context.UserLogs.Count());
            var signInUserId = "signInUserId";
            var result = Subject.CreateOrUpdateUserLog(signInUserId, GetUser(TestUserEmail_1));

            Assert.IsTrue(result);

            Assert.AreEqual(1, context.UserLogs.Count());

            Assert.IsNotNull(context.UserLogs.Include(x => x.User).First().User);
            Assert.IsNotNull(context.UserLogs.First().WelcomeEmailDateUtc);

            var firstUser = context.McUsers.First(x => x.Email == TestUserEmail_1);
            context.McUsers.Remove(firstUser);
            context.Save();

            Assert.AreEqual(1, context.UserLogs.Count());
            Assert.AreEqual(TestUserEmail_1, context.UserLogs.First().UserEmail);

            Assert.IsNull(context.UserLogs.Include(x => x.User).First().User);

            mockWelcomeEmailService.Verify(x => x.Send(It.IsAny<WelcomeEmailModel>()), Times.Once);

            var welcomeModel1 = new WelcomeEmailModel(firstUser);
            mockWelcomeEmailService.Verify(x => x.Send(
                It.Is<WelcomeEmailModel>(model =>
                    (model.EmailAddress == welcomeModel1.EmailAddress)
                    )
                ), Times.Once);

            var welcomeModel2 = new WelcomeEmailModel(GetUser(TestUserEmail_2, true));
            mockWelcomeEmailService.Verify(x => x.Send(It.Is<WelcomeEmailModel>(model =>
                    (model.EmailAddress == welcomeModel2.EmailAddress)
                    )), Times.Never);

            Assert.IsNotNull(context.UserLogs.First().WelcomeEmailDateUtc);
        }
    }
}
