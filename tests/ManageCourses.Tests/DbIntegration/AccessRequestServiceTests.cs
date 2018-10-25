using System;
using System.Collections.ObjectModel;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Services.AccessRequests;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.DbIntegration
{
    [TestFixture]
    [Category("Integration")]
    [Category("Integration_DB")]
    [Explicit]
    public class AccessRequestServiceTests : DbIntegrationTestBase
    {
        private AccessRequestService _system;
        private MockEmailService _emailService;

        protected override void Setup()
        {
            Context.AccessRequests.RemoveRange(Context.AccessRequests);

            Context.OrganisationUsers.RemoveRange(Context.OrganisationUsers);
            Context.Organisations.RemoveRange(Context.Organisations);
            Context.Users.RemoveRange(Context.Users);

            Context.Save();

            _emailService = new MockEmailService();

            _system = new AccessRequestService(Context, _emailService);
        }

        [Test]
        public void HappyPath()
        {
            Context.Users.Add(MakeSomeExistingUser());
            Context.Save();

            _system.LogAccessRequest(MakeSomeAccessRequest(), "joe@example.com");

            var savedRequest = Context.AccessRequests.Include(x => x.Requester).First();

            // Many, many asserts to cover all fields            
            Assert.AreEqual(MakeSomeAccessRequest().FirstName, savedRequest.FirstName);
            Assert.AreEqual(MakeSomeAccessRequest().LastName, savedRequest.LastName);
            Assert.AreEqual(MakeSomeAccessRequest().EmailAddress, savedRequest.EmailAddress);
            Assert.Less(0, savedRequest.Id);
            Assert.AreEqual(MakeSomeAccessRequest().Organisation, savedRequest.Organisation);
            Assert.AreEqual(MakeSomeAccessRequest().Reason, savedRequest.Reason);
            Assert.AreEqual(Context.Users.First().Id, savedRequest.RequesterId);
            Assert.AreEqual(Context.Users.First().Email, savedRequest.RequesterEmail);
            Assert.AreEqual(Context.Users.First().Email, savedRequest.Requester.Email);

            Assert.AreEqual(MakeSomeExistingUser().Email, _emailService.LastRequester.Email);
            Assert.AreEqual(MakeSomeExistingUser().FirstName, _emailService.LastRequester.FirstName);
            Assert.AreEqual(MakeSomeExistingUser().LastName, _emailService.LastRequester.LastName);

            Assert.AreEqual(MakeSomeExistingUser().OrganisationUsers.First().Organisation.Name, _emailService.LastRequester.OrganisationUsers.First().Organisation.Name);

            Assert.IsNull(_emailService.LastRequested);

            Assert.AreEqual(MakeSomeAccessRequest().FirstName, _emailService.LastAccessRequest.FirstName);
            Assert.AreEqual(MakeSomeAccessRequest().LastName, _emailService.LastAccessRequest.LastName);
            Assert.AreEqual(MakeSomeAccessRequest().EmailAddress, _emailService.LastAccessRequest.EmailAddress);
            Assert.AreEqual(MakeSomeAccessRequest().Organisation, _emailService.LastAccessRequest.Organisation);
            Assert.AreEqual(MakeSomeAccessRequest().Reason, _emailService.LastAccessRequest.Reason);

            Assert.AreEqual("joe@example.com", _emailService.LastAccessRequest.RequesterEmail);
        }

        [Test]
        public void RequestedUserExists()
        {
            var user = MakeSomeExistingUser();
            var user2 = MakeSomeOtherExistingUser();

            Context.Users.Add(user);
            Context.Users.Add(user2);
            Context.Save();

            _system.LogAccessRequest(MakeSomeAccessRequest(), "joe@example.com");

            Context.AccessRequests.Include(x => x.Requester).First();

            Assert.AreEqual(user2.FirstName, _emailService.LastRequested.FirstName);
            Assert.AreEqual(user2.LastName, _emailService.LastRequested.LastName);
            Assert.AreEqual(user2.Email, _emailService.LastRequested.Email);
        }

        [Test]
        public void UserDeletion_DoesntDeleteRequest()
        {
            var userToDelete = MakeSomeExistingUser();

            Context.Users.Add(userToDelete);
            Context.Save();

            _system.LogAccessRequest(MakeSomeAccessRequest(), "joe@example.com");

            Context.Users.Remove(userToDelete);
            Context.Save();

            var savedRequest = Context.AccessRequests.Include(x => x.Requester).First();

            Assert.Less(0, savedRequest.Id);
            Assert.IsNull(savedRequest.RequesterId);
            Assert.IsNull(savedRequest.Requester);
            Assert.AreEqual("joe@example.com", savedRequest.RequesterEmail);
        }

        [Test]
        public void RequesterEmailIsCaseInsensitive()
        {
            var user = MakeSomeExistingUser();

            Context.Users.Add(user);
            Context.Save();

            _system.LogAccessRequest(MakeSomeAccessRequest(), "JoE@eXaMpLE.CoM");

            var savedRequest = Context.AccessRequests.Include(x => x.Requester).First();

            Assert.Less(0, savedRequest.Id);
            Assert.AreEqual("joe@example.com", savedRequest.RequesterEmail);
            Assert.AreEqual("joe@example.com", savedRequest.Requester.Email);
        }

        [Test]
        public void RequestedEmailIsCaseInsensitive()
        {
            var user = MakeSomeExistingUser();
            var user2 = MakeSomeOtherExistingUser();

            Context.Users.Add(user);
            Context.Users.Add(user2);
            Context.Save();

            var accessRequest = MakeSomeAccessRequest();
            accessRequest.EmailAddress = "jAnE@eXaMpLe.CoM";
            _system.LogAccessRequest(accessRequest, "joe@example.com");

            var savedRequest = Context.AccessRequests.Include(x => x.Requester).First();

            Assert.Less(0, savedRequest.Id);
            Assert.AreEqual("jane@example.com", _emailService.LastRequested.Email);
        }

        [Test]
        public void NonexistentRequester_Throws()
        {
            try
            {
                _system.LogAccessRequest(MakeSomeAccessRequest(), "joe@example.com");
            }
            catch
            {
                Assert.AreEqual(0, Context.AccessRequests.Count());
                Assert.IsNull(_emailService.LastAccessRequest);
                return;
            }
            Assert.Fail("Should have thrown");
        }

        [Test]
        public void EmailError_RollsBackAccessRequest()
        {
            Context.Users.Add(MakeSomeExistingUser());
            Context.Save();

            _system = new AccessRequestService(Context, new ErroringEmailService());
            try
            {
                _system.LogAccessRequest(MakeSomeAccessRequest(), "joe@example.com");
            }
            catch
            {
                Assert.AreEqual(0, Context.AccessRequests.Count());
                return;
            }

            Assert.Fail("Should have thrown");
        }

        private User MakeSomeExistingUser()
        {
            var res = new User()
            {
                FirstName = "Joe",
                LastName = "Bloggs",
                Email = "joe@example.com"
            };

            res.OrganisationUsers = new Collection<OrganisationUser> {
                    new OrganisationUser
                    {
                        User = res,
                        Organisation = new Organisation {
                            Name = "Joe's school",
                            OrgId = "123"
                        }
                    }
                };

            return res;
        }

        private static User MakeSomeOtherExistingUser()
        {
            return new User()
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane@example.com"
            };
        }

        private static Api.Model.AccessRequest MakeSomeAccessRequest()
        {
            return new Api.Model.AccessRequest
            {
                FirstName = "Jane",
                LastName = "Doe",
                EmailAddress = "jane@example.com",
                Organisation = "Joe and Jane's happy school",
                Reason = "Jane is doing all the prose"
            };
        }

        private class ErroringEmailService : IAccessRequestEmailService
        {
            public void SendAccessRequestEmailToSupport(AccessRequest accessRequest, User requester, User requestedOrNull)
            {
                throw new SuperUnexpectedException();
            }
        }

        private class SuperUnexpectedException : Exception
        {

        }

        private class MockEmailService : IAccessRequestEmailService
        {
            public AccessRequest LastAccessRequest;
            public User LastRequester;
            public User LastRequested;

            public void SendAccessRequestEmailToSupport(AccessRequest accessRequest, User requester, User requestedOrNull)
            {
                LastAccessRequest = accessRequest;
                LastRequester = requester;
                LastRequested = requestedOrNull;
            }
        }
    }
}