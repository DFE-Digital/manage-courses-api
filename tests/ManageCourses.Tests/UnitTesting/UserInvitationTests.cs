using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Services.Email;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    /// <summary>
    /// Unit test verifying the behaviour of the UserService
    /// around inviting new users to manage courses.
    /// </summary>
    [TestFixture]
    public class UserInvitationTests
    {
        private UserService _userService;
        private IClock _clock;
        private Mock<IManageCoursesDbContext> _context;

        // non-existent user, no org specified - should fail
        // existing user, no org specified - should fail

        // non-existent user, unknown org specified - should fail
        // existing user, unknown org specified - should fail
        // non-existent user, existing org specified - should attach and invite
        // existing user non-existent org - should fail

        // should update name in McUsers
        // invite should have name from invite request not from stale McUser data

            // what happens if already invited user is in the list???

        [SetUp]
        public void Setup()
        {
            _context = new Mock<IManageCoursesDbContext>();
            _clock = new Mock<IClock>().Object;
            _userService = new UserService(_context.Object, new Mock<IWelcomeEmailService>().Object, _clock);
        }


        [Test]
        public void Adds_NewUser()
        {
            // known user
            // unknown user
        }

        [Test]
        public void LinksUsersToOrgs()
        {
            // known user
            // unknown user
            // no-op if org already associated
        }

        [Test]
        public void InviteEmailSent()
        {
            // known user
            // unknown user
            // check name matches request and not stale user data
            // check not sent more than once if called again
        }

        [Test]
        public void InviteDateRecorded()
        {
        }

        [Test]
        public void ThrowFor_UnknownOrg()
        {
            // known user
            // unknown user
        }

        [Test]
        public void ThrowFor_NewUser_WithoutOrg()
        {
            // known user
            // unknown user
        }
    }
}
