using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Controllers;
using GovUk.Education.ManageCourses.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting.Controllers
{
    [TestFixture]
    public class InviteControllerTests
    {
        [Test]
        public void Invite_Returns200()
        {
            var inviteServiceMock = new Mock<IInviteService>(); // default mock is enough to accecpt call to Invite() without error
            var inviteController = new InviteController(inviteServiceMock.Object);
            var result = inviteController.Index("foo@example.org");
            result.Should().BeOfType<OkResult>();
        }
    }
}
