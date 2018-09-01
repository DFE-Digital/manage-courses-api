using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Exceptions;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Services.Email;
using GovUk.Education.ManageCourses.Api.Services.Email.Model;
using GovUk.Education.ManageCourses.Api.Services.Users;
using GovUk.Education.ManageCourses.Domain.Models;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.DbIntegration
{
    [TestFixture]
    [Category("Integration")]
    [Category("Integration_DB")]
    [Explicit]
    public class UserServiceTests : DbIntegrationTestBase
    {
        private IUserService _userService;
        private Mock<IWelcomeEmailService> _mockWelcomeEmailService;
        private DateTime _mockTime = new DateTime(1978, 1, 2, 3, 4, 5, 7);
        private McUser _testUserBob;
        private McUser _userTestUserFrank;

        protected override void Setup()
        {
            _testUserBob = new McUser
            {
                FirstName = "1.Spongebob",
                LastName = "Squarepants",
                Email = "bob@example.org",
            };
            _userTestUserFrank = new McUser
            {
                FirstName = "Franky",
                LastName = "Sinatra",
                Email = "frank@example.org",
            };
            Context.AddMcUser(_testUserBob);
            Context.AddMcUser(_userTestUserFrank);
            Context.SaveChanges();

            _mockWelcomeEmailService = new Mock<IWelcomeEmailService>();

            var mockClock = new Mock<IClock>();
            mockClock.SetupGet(c => c.UtcNow).Returns(() => _mockTime);

            _userService = new UserService(Context, _mockWelcomeEmailService.Object, mockClock.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            Context.Remove(_testUserBob);
            Context.Remove(_userTestUserFrank);
            Context.Save();
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
            Func<Task> signIn = async () => { await _userService.UserSignedInAsync("abc", unknownUser); };
            signIn.Should().Throw<McUserNotFoundException>($"{unknownUser.Email} / {unknownUser.Subject} does not exist in McUsers");
        }

        [Test]
        public void SignInTest()
        {
            var firstSignInTime = new DateTime(2017, 12, 31, 23, 59, 59);
            _mockTime = firstSignInTime;

            const string bobSubject = "2C4B4170-8979-444F-8D44-DC6DE22BEABF";
            var userDetails1 = new JsonUserDetails
            {
                Email = _testUserBob.Email,
                GivenName = "2.Bobby",
                FamilyName = "Charlton", // different to contents of McUsers to check it gets updated
                Subject = bobSubject,
            };

            // test a realistic journey, validating the state of the data at each step

            // bob signs in for the first time
            _userService.UserSignedInAsync("abc", userDetails1);
            // check user data updated from claims and timestamps have been set
            CheckUserDataUpdated(_testUserBob, userDetails1);
            _testUserBob.FirstLoginDateUtc.Should().Be(firstSignInTime);
            _testUserBob.LastLoginDateUtc.Should().Be(firstSignInTime);
            // check welcome email sent & logged
            _testUserBob.WelcomeEmailDateUtc.Should().Be(firstSignInTime);

            var welcomeModelBob = new WelcomeEmailModel(_testUserBob);
            _mockWelcomeEmailService.Verify(
                x => x.Send(It.Is<WelcomeEmailModel>(model => (model.EmailAddress == welcomeModelBob.EmailAddress))),
                Times.Once);

            var secondSignInTime = _mockTime.AddHours(8);
            _mockTime = secondSignInTime;

            // bob signs in again, with a new name & email
            // this checks that we are now relying on the sign-in guid and not the email address,
            // and also that the email address gets updated
            var userDetails2 = new JsonUserDetails
            {
                Email = _testUserBob.Email,
                //Email = "sirbob@example.org"; // todo: check for email address changes, blocked by use of email as an FK
                GivenName = "3.Sir Bob",
                FamilyName = "Charlton the legend",
                Subject = bobSubject,
            };
            _userService.UserSignedInAsync("def", userDetails2); // would throw if it couldn't find the McUser entry
            // check user data updated from claims and timestamps have been set
            CheckUserDataUpdated(_testUserBob, userDetails2);
            _testUserBob.LastLoginDateUtc.Should().Be(secondSignInTime);
            _testUserBob.Sessions.Count.Should().Be(2);
            _testUserBob.Sessions.Single(x => x.AccessToken == "abc").Subject.Should().Be(bobSubject);
            _testUserBob.Sessions.Single(x => x.AccessToken == "abc").CreatedUtc.Should().BeAfter(DateTime.UtcNow.AddSeconds(-1));
            _testUserBob.Sessions.Single(x => x.AccessToken == "def").Subject.Should().Be(bobSubject);            
            _testUserBob.Sessions.Single(x => x.AccessToken == "def").CreatedUtc.Should().BeAfter(DateTime.UtcNow.AddSeconds(-1));
            // check original timestamps have not been altered
            _testUserBob.FirstLoginDateUtc.Should().Be(firstSignInTime);
            _testUserBob.WelcomeEmailDateUtc.Should().Be(firstSignInTime);

            // check only one email was sent
            _mockWelcomeEmailService.Verify(x => x.Send(It.IsAny<WelcomeEmailModel>()), Times.Once);
        }

        private void CheckUserDataUpdated(McUser mcUser, JsonUserDetails jsonUserDetails)
        {
            Thread.Sleep(100); // EF Core proxy object is returning stale data. Delay allows it to settle. Suggestions welcome!
            var mcUserFirstName = mcUser.FirstName; // seems to be a race condition or something here, keep getting stale value assigned
            mcUserFirstName.Should().Be(jsonUserDetails.GivenName);
            mcUser.LastName.Should().Be(jsonUserDetails.FamilyName);
            mcUser.Email.Should().Be(jsonUserDetails.Email);
        }
    }
}
