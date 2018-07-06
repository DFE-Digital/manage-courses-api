﻿using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using GovUk.Education.ManageCourses.Tests.Integration.DatabaseAccess;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
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
        public IManageCoursesDbContext Context = null;

        private const string TestUserEmail_1 = "email_1@test-manage-courses.gov.uk";
        private const string TestUserEmail_2 = "email_2@test-manage-courses.gov.uk";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Context = this.GetContext();

            Context.AddMcUser(new McUser
            {
                FirstName = "FirstName_1",
                LastName = "LastName_1",
                Email = TestUserEmail_1
            });

            Context.Save();

        }
        [SetUp]
        public void Setup()
        {
            Subject = new UserLogService(this.Context);
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var item in Context.UserLogs)
            {
                var x = ((DbContext)Context).Entry(item);
                this.entitiesToCleanUp.Add(x);
            }
        }

        [Test]
        public void UpdateUserLog()
        {
            var signInUserId = "signInUserId";
            var firstLoginDateUtc = DateTime.UtcNow.AddDays(-10);

            var existing = Context.UserLogs.Add(new UserLog { SignInUserId = "signInUserId", FirstLoginDateUtc = firstLoginDateUtc, LastLoginDateUtc = firstLoginDateUtc, User = Context.McUsers.First()  });

            Context.Save();
            var email = TestUserEmail_1;
            
            var result = Subject.CreateOrUpdateUserLog(signInUserId, email);

            Assert.IsTrue(result);

            Assert.AreEqual(1, Context.UserLogs.Count());

            var saved = Context.UserLogs.First();
            Assert.IsNotNull(saved.User);
            Assert.AreEqual(firstLoginDateUtc, saved.FirstLoginDateUtc);
            Assert.AreNotEqual(firstLoginDateUtc, saved.LastLoginDateUtc);
        }

        [Test]
        public void CreateUserLog()
        {
            var email = TestUserEmail_1;
            var signInUserId = "signInUserId";
            var result = Subject.CreateOrUpdateUserLog(signInUserId, email);

            Assert.IsTrue(result);

            Assert.AreEqual(1, Context.UserLogs.Count());

            var result2 = Subject.CreateOrUpdateUserLog(signInUserId + 2, TestUserEmail_2);

            Assert.IsTrue(result2);

            Assert.AreEqual(2, Context.UserLogs.Count());
        }


        [Test]
        public void CreateOrUpdateUserLog()
        {
            var email = TestUserEmail_1;
            var signInUserId = "signInUserId";
            var result = Subject.CreateOrUpdateUserLog(signInUserId, email);

            Assert.IsTrue(result);

            Assert.AreEqual(1, Context.UserLogs.Count());

            var result2 = Subject.CreateOrUpdateUserLog(signInUserId, TestUserEmail_2);

            Assert.IsTrue(result2);

            Assert.AreEqual(1, Context.UserLogs.Count());
        }
    }
}