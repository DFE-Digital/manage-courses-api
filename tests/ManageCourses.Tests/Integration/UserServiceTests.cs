using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Tests.Integration.DatabaseAccess;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
namespace GovUk.Education.ManageCourses.Tests.Integration
{
    [TestFixture]
    [Category("Integration")]
    [Category("Integration_DB")]
    [Explicit]
    public class UserServiceTests : ManageCoursesDbContextIntegrationBase
    {
        private IUserService _userService;
        private Mock<IWelcomeEmailService> _mockWelcomeEmailService;
        private DateTime _mockTime = new DateTime(1978,1,2,3,4,5,7);
        private McUser _testUserBob;
        private McUser _userTestUserFrank;

        [SetUp]
        public void Setup()
        {
            _testUserBob = new McUser
            {
                FirstName = "Spongebob",
                LastName = "Squarepants",
                Email = "bob@example.org",
            };
            _userTestUserFrank = new McUser
            {
                FirstName = "Franky",
                LastName = "Sinatra",
                Email = "frank@example.org",
            };
            context.AddMcUser(_testUserBob);
            context.AddMcUser(_userTestUserFrank);
            context.SaveChanges();

            _mockWelcomeEmailService = new Mock<IWelcomeEmailService>();

            var mockClock = new Mock<IClock>();
            mockClock.SetupGet(c => c.UtcNow).Returns(() => _mockTime);

            _userService = new UserService(context, _mockWelcomeEmailService.Object, mockClock.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            context.Remove(_testUserBob);
            context.Remove(_userTestUserFrank);
            context.Save();
        }

        [Test]
        public void UnknownUserSignInShouldThrow()
        {
            var unknownUser = new JsonUserDetails
            {
                Email = "who@example.org",
                FamilyName = "Who",
                GivenName = "The",
                Subject = "673535D4-3CB3-4A1D-B1F0-B0FFB787CECF",
            };
            Func<Task> signIn = async () => { await _userService.UserSignedInAsync(unknownUser); };
            signIn.Should().Throw<UnknownMcUserException>($"{unknownUser.Email} / {unknownUser.Subject} does not exist in McUsers");
        }

        [Test]
        public void SignInTest()
        {
            var firstSignInTime = new DateTime(2017, 12, 31, 23, 59, 59);
            _mockTime = firstSignInTime;

            const string bobSubject = "2C4B4170-8979-444F-8D44-DC6DE22BEABF";
            var jsonUserDetails = new JsonUserDetails
            {
                Email = _testUserBob.Email,
                FamilyName = "Charlton", // different to contents of McUsers to check it gets updated
                GivenName = "Bobby",
                Subject = bobSubject,
            };

            // test a realistic journey, validating the state of the data at each step

            // bob signs in for the first time
            _userService.UserSignedInAsync(jsonUserDetails);
            // check user data updated from claims and timestamps have been set
            CheckUserDataUpdated(_testUserBob, jsonUserDetails);
            _testUserBob.FirstLoginDateUtc.Should().Be(firstSignInTime);
            _testUserBob.LastLoginDateUtc.Should().Be(firstSignInTime);
            // check welcome email sent & logged
            _testUserBob.WelcomeEmailDateUtc.Should().Be(firstSignInTime);
            _mockWelcomeEmailService.Verify(x => x.Send(_testUserBob), Times.Once);

            var secondSignInTime = _mockTime.AddHours(8);
            _mockTime = secondSignInTime;

            // bob signs in again, with a new name & email
            // this checks that we are now relying on the sign-in guid and not the email address,
            // and also that the email address gets updated
            //jsonUserDetails.Email = "sirbob@example.org"; // todo: check for email address changes, blocked by use of email as an FK
            jsonUserDetails.GivenName = "Sir Bob";
            jsonUserDetails.FamilyName = "Charlton the legend";
            _userService.UserSignedInAsync(jsonUserDetails); // would throw if it couldn't find the McUser entry
            // check user data updated from claims and timestamps have been set
            CheckUserDataUpdated(_testUserBob, jsonUserDetails);
            _testUserBob.LastLoginDateUtc.Should().Be(secondSignInTime);
            // check original timestamps have not been altered
            _testUserBob.FirstLoginDateUtc.Should().Be(firstSignInTime);
            _testUserBob.WelcomeEmailDateUtc.Should().Be(firstSignInTime);

            // check only one email was sent
            _mockWelcomeEmailService.Verify(x => x.Send(It.IsAny<McUser>()), Times.Once);
        }

        private void CheckUserDataUpdated(McUser mcUser, JsonUserDetails jsonUserDetails)
        {
            var mcUserFirstName = mcUser.FirstName; // seems to be a race condition or something here, keep getting stale value assigned
            mcUserFirstName.Should().Be(jsonUserDetails.GivenName);
            mcUser.LastName.Should().Be(jsonUserDetails.FamilyName);
            mcUser.Email.Should().Be(jsonUserDetails.Email);
        }
    }
}
