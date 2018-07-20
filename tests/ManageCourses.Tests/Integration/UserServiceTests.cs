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
        private DateTime _mockTime;
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
            mockClock.SetupGet(c => c.UtcNow).Returns(_mockTime);

            _userService = new UserService(context, _mockWelcomeEmailService.Object, mockClock.Object);
        }

        [TearDown]
        public override void TearDown()
        {
            foreach (var item in context.McUsers)
            {
                var x = ((DbContext)context).Entry(item);
                this.entitiesToCleanUp.Add(x);
            }
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
            _mockTime = new DateTime(2017, 12, 31, 23, 59, 59);

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
            _testUserBob.FirstLoginDateUtc.Should().Be(_mockTime);
            _testUserBob.LastLoginDateUtc.Should().Be(_mockTime);
            // check welcome email sent & logged
            _testUserBob.WelcomeEmailDateUtc.Should().Be(_mockTime);
            _mockWelcomeEmailService.Verify(x => x.Send(_testUserBob), Times.Once);

            var mockLaterTime = _mockTime.AddHours(8);

            // bob signs in again, with a new name & email
            // this checks that we are now relying on the sign-in guid and not the email address,
            // and also that the email address gets updated
            jsonUserDetails.Email = "sirbob@example.org";
            jsonUserDetails.GivenName = "Sir Bob";
            jsonUserDetails.FamilyName = "Charlton the legend";
            _userService.UserSignedInAsync(jsonUserDetails); // would throw if it couldn't find the McUser entry
            // check user data updated from claims and timestamps have been set
            CheckUserDataUpdated(_testUserBob, jsonUserDetails);
            _testUserBob.LastLoginDateUtc.Should().Be(mockLaterTime);
            // check original timestamps have not been altered
            _testUserBob.FirstLoginDateUtc.Should().Be(_mockTime);
            _testUserBob.WelcomeEmailDateUtc.Should().Be(_mockTime);

            // check only one email was sent
            _mockWelcomeEmailService.Verify(x => x.Send(It.IsAny<McUser>()), Times.Once);
        }

        private void CheckUserDataUpdated(McUser user, JsonUserDetails jsonUserDetails)
        {
            user.FirstName.Should().Be(jsonUserDetails.GivenName);
            user.LastName.Should().Be(jsonUserDetails.FamilyName);
            user.Email.Should().Be(jsonUserDetails.Email);
        }
    }
}
