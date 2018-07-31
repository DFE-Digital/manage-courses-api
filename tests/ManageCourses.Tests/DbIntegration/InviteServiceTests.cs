﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Exceptions;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Services.Email;
using GovUk.Education.ManageCourses.Api.Services.Email.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.DbIntegration
{
    [TestFixture]
    [Category("Integration")]
    [Category("Integration_DB")]
    [Explicit]
    public class InviteServiceTests : DbIntegrationTestBase
    {
        private DateTime _mockTime = new DateTime(1977, 1, 2, 3, 4, 5, 7);
        private Mock<IInviteEmailService> _mockInviteEmailService;
        private InviteService _inviteService;

        [OneTimeSetUp]
        public void Setup()
        {
            var mockUsers = new List<McUser>
            {
                new McUser{Email = "not.me@example.org"},
            };
            Context.McUsers.AddRange(mockUsers);
            Context.SaveChanges();
            var mockClock = new Mock<IClock>();
            mockClock.SetupGet(c => c.UtcNow).Returns(() => _mockTime);
            _mockInviteEmailService = new Mock<IInviteEmailService>();
            _inviteService = new InviteService(_mockInviteEmailService.Object, Context, mockClock.Object);
        }

        [Test]
        public void InviteEmailSent()
        {
            const string email = "janet@example.org";
            AddUser(email);
            _inviteService.Invite(email);
            _mockInviteEmailService.Verify(
                x => x.Send(It.Is<InviteEmailModel>(model => (model.EmailAddress == email))),
                Times.Once);
        }

        [Test]
        public void InviteDateRecorded()
        {
            // arrange
            const string email = "john@example.org";
            var user = AddUser(email);
            var inviteTime = new DateTime(2020, 12, 31, 8, 7, 6);
            _mockTime = inviteTime;

            // act
            _inviteService.Invite(email);

            // assert
            var userAfter = EfCacheBuster(user);
            userAfter.InviteDateUtc.Should().Be(inviteTime);
        }

        [Test]
        public void InviteEmailNotResent()
        {
            const string email = "jack@example.org";
            var user = AddUser(email);
            var originalInviteTime = new DateTime(2020, 12, 31, 8, 7, 7);

            _mockTime = originalInviteTime;
            _inviteService.Invite(email);

            _mockTime = originalInviteTime.AddSeconds(1);
            _inviteService.Invite(email);

            // assert
            var userAfter = EfCacheBuster(user);
            userAfter.InviteDateUtc.Should().Be(originalInviteTime);
        }

        [Test]
        public void ThrowsForUnknownMcUser()
        {
            Assert.Throws<McUserNotFoundException>(() => _inviteService.Invite("jamie-oliver@example.org"));
        }

        private McUser AddUser(string email)
        {
            var mcUser = new McUser { Email = email };
            Context.McUsers.Add(mcUser);
            Context.SaveChanges();
            return mcUser;
        }

        private TEntity EfCacheBuster<TEntity>(TEntity staleEntity) where TEntity : McBase
        {
            // Cache-bust EF https://stackoverflow.com/questions/46205114/how-to-refresh-an-entity-framework-core-dbcontext#51290890
            // "I believe that this leaves the entry in the cache but causes
            //  the DbContext to remove and replace it when you actually load
            //  the entry in the future. What matters is that it causes a future
            //  load to return a freshly loaded entity instance to your code."
            Context.Entry(staleEntity).State = EntityState.Detached;
            var userAfter = Context.Find<TEntity>(staleEntity.Id);
            return userAfter;
        }
    }
}