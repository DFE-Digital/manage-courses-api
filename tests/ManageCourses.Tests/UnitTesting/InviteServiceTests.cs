using System.Collections.Generic;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Services.Email;
using GovUk.Education.ManageCourses.Api.Services.Email.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    public class InviteServiceTests
    {
        private const string JanetEmail = "janet@example.org";
        private IClock _clock;
        private Mock<ManageCoursesDbContext> _context;
        private Mock<IInviteEmailService> _mockInviteEmailService;

        [SetUp]
        public void Setup()
        {
            _context = new Mock<ManageCoursesDbContext>();
            var mockUsers = new List<McUser>
            {
                new McUser{Email = JanetEmail}
            };
            _context.Setup(c => c.McUsers).ReturnsDbSet(mockUsers);
            _clock = new Mock<IClock>().Object;
            _mockInviteEmailService = new Mock<IInviteEmailService>();
        }

        [Test]
        public void InviteEmailSent()
        {
            var inviteService = new InviteService(_mockInviteEmailService.Object, _context.Object);
            inviteService.Invite(JanetEmail);
            _mockInviteEmailService.Verify(
                x => x.Send(It.Is<InviteEmailModel>(model => (model.EmailAddress == JanetEmail))),
                Times.Once);
        }

        [Test]
        public void InviteDateRecorded()
        {
        }

        [Test]
        public void InviteEmailNotResent()
        {
        }

        [Test]
        public void ThrowsForUnknownMcUser()
        {
        }
    }
}
