using System;
using System.Collections.ObjectModel;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Data;
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
        private AccessRequestService System;
        private MockEmailService EmailService;
        
        [SetUp]
        public void Setup()
        {
            Context.AccessRequests.RemoveRange(Context.AccessRequests);
            
            Context.McOrganisationUsers.RemoveRange(Context.McOrganisationUsers);
            Context.McOrganisations.RemoveRange(Context.McOrganisations);
            Context.McUsers.RemoveRange(Context.McUsers);

            Context.Save();

            EmailService = new MockEmailService();

            System = new AccessRequestService(Context, EmailService);
        }

        [Test]
        public void HappyPath()
        {
            Context.AddMcUser(MakeSomeExistingUser());
            Context.Save();

            System.LogAccessRequest(MakeSomeAccessRequest(), "joe@example.com");

            var savedRequest = Context.AccessRequests.Include(x=>x.Requester).First();

            // Many, many asserts to cover all fields            
            Assert.AreEqual(MakeSomeAccessRequest().FirstName, savedRequest.FirstName);
            Assert.AreEqual(MakeSomeAccessRequest().LastName, savedRequest.LastName);
            Assert.AreEqual(MakeSomeAccessRequest().EmailAddress, savedRequest.EmailAddress);
            Assert.Less(0, savedRequest.Id);
            Assert.AreEqual(MakeSomeAccessRequest().Organisation, savedRequest.Organisation);
            Assert.AreEqual(MakeSomeAccessRequest().Reason, savedRequest.Reason);
            Assert.AreEqual(Context.McUsers.First().Id, savedRequest.RequesterId);
            Assert.AreEqual(Context.McUsers.First().Email, savedRequest.RequesterEmail);
            Assert.AreEqual(Context.McUsers.First().Email, savedRequest.Requester.Email);

            Assert.AreEqual(MakeSomeExistingUser().Email, EmailService.lastRequester.Email);            
            Assert.AreEqual(MakeSomeExistingUser().FirstName, EmailService.lastRequester.FirstName);            
            Assert.AreEqual(MakeSomeExistingUser().LastName, EmailService.lastRequester.LastName);

            Assert.AreEqual(MakeSomeExistingUser().McOrganisationUsers.First().McOrganisation.Name, EmailService.lastRequester.McOrganisationUsers.First().McOrganisation.Name);

            Assert.IsNull(EmailService.lastRequested);

            Assert.AreEqual(MakeSomeAccessRequest().FirstName, EmailService.lastAccessRequest.FirstName);
            Assert.AreEqual(MakeSomeAccessRequest().LastName, EmailService.lastAccessRequest.LastName);
            Assert.AreEqual(MakeSomeAccessRequest().EmailAddress, EmailService.lastAccessRequest.EmailAddress);
            Assert.AreEqual(MakeSomeAccessRequest().Organisation, EmailService.lastAccessRequest.Organisation);
            Assert.AreEqual(MakeSomeAccessRequest().Reason, EmailService.lastAccessRequest.Reason);
            
            Assert.AreEqual("joe@example.com", EmailService.lastAccessRequest.RequesterEmail);           
        }

        [Test]
        public void RequestedUserExists()
        {            
            var user =  MakeSomeExistingUser();
            var user2 = MakeSomeOtherExistingUser();
            
            Context.AddMcUser(user);
            Context.AddMcUser(user2);
            Context.Save();

            System.LogAccessRequest(MakeSomeAccessRequest(), "joe@example.com");

            var savedRequest = Context.AccessRequests.Include(x=>x.Requester).First();

            Assert.AreEqual(user2.FirstName, EmailService.lastRequested.FirstName);
            Assert.AreEqual(user2.LastName, EmailService.lastRequested.LastName);
            Assert.AreEqual(user2.Email, EmailService.lastRequested.Email);
        }

        [Test]
        public void UserDeletion_DoesntDeleteRequest()
        {
            var userToDelete = MakeSomeExistingUser();

            Context.AddMcUser(userToDelete);
            Context.Save();

            System.LogAccessRequest(MakeSomeAccessRequest(), "joe@example.com");

            Context.McUsers.Remove(userToDelete);
            Context.Save();

            var savedRequest = Context.AccessRequests.Include(x=>x.Requester).First();

            Assert.Less(0, savedRequest.Id);
            Assert.IsNull(savedRequest.RequesterId);
            Assert.IsNull(savedRequest.Requester);
            Assert.AreEqual("joe@example.com", savedRequest.RequesterEmail);
        }

        [Test]
        public void RequesterEmailIsCaseInsensitive()
        {
            var user =  MakeSomeExistingUser();

            Context.AddMcUser(user);
            Context.Save();

            System.LogAccessRequest(MakeSomeAccessRequest(), "JoE@eXaMpLE.CoM");

            var savedRequest = Context.AccessRequests.Include(x=>x.Requester).First();

            Assert.Less(0, savedRequest.Id);
            Assert.AreEqual("joe@example.com", savedRequest.RequesterEmail);
            Assert.AreEqual("joe@example.com", savedRequest.Requester.Email);
        }

        [Test]
        public void RequestedEmailIsCaseInsensitive()
        {
            var user =  MakeSomeExistingUser();
            var user2 = MakeSomeOtherExistingUser();            

            Context.AddMcUser(user);
            Context.AddMcUser(user2);
            Context.Save();

            var accessRequest = MakeSomeAccessRequest();
            accessRequest.EmailAddress = "jAnE@eXaMpLe.CoM";
            System.LogAccessRequest(accessRequest, "joe@example.com");

            var savedRequest = Context.AccessRequests.Include(x=>x.Requester).First();

            Assert.Less(0, savedRequest.Id);
            Assert.AreEqual("jane@example.com", EmailService.lastRequested.Email);
        }

        [Test]
        public void NonexistentRequester_Throws()
        {
            try 
            {
                System.LogAccessRequest(MakeSomeAccessRequest(), "joe@example.com");
            }
            catch
            {
                Assert.AreEqual(0, Context.AccessRequests.Count());
                Assert.IsNull(EmailService.lastAccessRequest);
                return;
            }
            Assert.Fail("Should have thrown");
        }

        [Test]
        public void EmailError_RollsBackAccessRequest()
        {
            Context.AddMcUser(MakeSomeExistingUser());
            Context.Save();

            System = new AccessRequestService(Context, new ErroringEmailService());
            try 
            {
                System.LogAccessRequest(MakeSomeAccessRequest(), "joe@example.com");
            }
            catch
            {
                Assert.AreEqual(0, Context.AccessRequests.Count());
                return;
            }

            Assert.Fail("Should have thrown");
        }

        private McUser MakeSomeExistingUser() 
        {
            var res = new McUser() {
                FirstName = "Joe",
                LastName = "Bloggs",
                Email = "joe@example.com"
            };

            res.McOrganisationUsers = new Collection<McOrganisationUser> {
                    new McOrganisationUser 
                    {
                        McUser = res,
                        McOrganisation = new McOrganisation {
                            Name = "Joe's school",
                            OrgId = "123"
                        }                        
                    }
                };
            
            return res;
        }

        private McUser MakeSomeOtherExistingUser() 
        {
            return new McUser() {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane@example.com"
            };
        }

        private Api.Model.AccessRequest MakeSomeAccessRequest()
        {
            return new Api.Model.AccessRequest {
                FirstName = "Jane",
                LastName = "Doe",
                EmailAddress = "jane@example.com",
                Organisation = "Joe and Jane's happy school",
                Reason = "Jane is doing all the prose"
            };
        }

        private class ErroringEmailService : IAccessRequestEmailService
        {
            public void SendAccessRequestEmailToSupport(AccessRequest accessRequest, McUser requester, McUser requestedOrNull)
            {
                throw new SuperUnexpectedException();
            }
        }

        private class SuperUnexpectedException : Exception
        {

        }

        private class MockEmailService : IAccessRequestEmailService
        {
            public AccessRequest lastAccessRequest = null;
            public McUser lastRequester = null;
            public McUser lastRequested = null;

            public int called = 0;
            public void SendAccessRequestEmailToSupport(AccessRequest accessRequest, McUser requester, McUser requestedOrNull)
            {
                called++;
                lastAccessRequest = accessRequest;
                lastRequester = requester;
                lastRequested = requestedOrNull;
            }
        }
    }
}