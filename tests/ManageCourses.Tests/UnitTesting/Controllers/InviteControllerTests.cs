using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Controllers;
using GovUk.Education.ManageCourses.Api.Exceptions;
using GovUk.Education.ManageCourses.Api.Services.Invites;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting.Controllers
{
    [TestFixture]
    public class InviteControllerTests
    {
        private Mock<IInviteService> _inviteServiceMock;
        private InviteController _inviteController;

        [SetUp]
        public void SetUp()
        {
            _inviteServiceMock = new Mock<IInviteService>();
            _inviteController = new InviteController(_inviteServiceMock.Object);
        }

        [Test]
        public void Invite_Returns200()
        {
            // act
            var result = _inviteController.Index("foo@example.org");

            // assert
            result.Should().BeOfType<OkResult>();
        }

        [Test]
        public void Invite_UnknownUser_ReturnsBadRequest()
        {
            // arrange
            _inviteServiceMock.Setup(s => s.Invite(It.IsAny<string>())).Throws<McUserNotFoundException>();

            // act
            var result = _inviteController.Index("foo@example.org");

            // assert
            result.Should().BeOfType<BadRequestResult>();
        }
    }
}
