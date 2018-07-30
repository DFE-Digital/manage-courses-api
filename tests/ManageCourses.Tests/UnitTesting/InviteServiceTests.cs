using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Services.Email;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    public class InviteServiceTests
    {
        private UserService _userService;
        private IClock _clock;
        private Mock<IManageCoursesDbContext> _context;

        [SetUp]
        public void Setup()
        {
            _context = new Mock<IManageCoursesDbContext>();
            _clock = new Mock<IClock>().Object;
            _userService = new UserService(_context.Object, new Mock<IWelcomeEmailService>().Object, _clock);
        }

        [Test]
        public void InviteEmailSent()
        {
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
