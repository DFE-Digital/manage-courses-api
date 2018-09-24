
using System.Collections.ObjectModel;
using System.Linq;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Controllers;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Tests.DbIntegration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.Controllers
{
    [TestFixture]
    public class AdminControllerTests : DbIntegrationTestBase
    {
        [Test]
        public void ActionAccessRequest()
        {
            SaveExampleDataToContext();

            var id = Context.AccessRequests.Single().Id;

            var res = new AdminController(Context).ActionAccessRequest(id);

            var users = Context.McUsers.Include(x => x.McOrganisationUsers).ToList();

            (res as StatusCodeResult).StatusCode.Should().Be(200);
            users.Count.Should().Be(2);
            var recipient = users.Single(x => x.Email == "recipient@example.com");

            recipient.FirstName.Should().Be("RecipientFirstName");
            recipient.LastName.Should().Be("RecipientLastName");
            recipient.McOrganisationUsers.Single().OrgId.Should().Be("123");
            recipient.McOrganisationUsers.Single().McOrganisation.Name.Should().Be("TheOrg");
            Context.AccessRequests.Single().Status.Should().Be(AccessRequest.RequestStatus.Completed);
        }
        
        [Test]
        public void ActionAccessRequest_UnknownAccessRequest()
        {
            var res = new AdminController(Context).ActionAccessRequest(999);
            (res as StatusCodeResult).StatusCode.Should().Be(404);
        } 

        [Test]
        public void ActionAccessRequest_RepeatActioning()
        {
            SaveExampleDataToContext();
            var id = Context.AccessRequests.Single().Id;

            var controller = new AdminController(Context);

            controller.ActionAccessRequest(id);
            controller.ActionAccessRequest(id);

            var res = controller.ActionAccessRequest(id);
        

            Context.McUsers.Count().Should().Be(2);
            Context.McUsers.Include(x => x.McOrganisationUsers).Last().McOrganisationUsers.Count().Should().Be(1);
        }

        [Test]
        public void ActionManualAccessRequest()
        {
            SaveExampleDataToContext();
            var controller = new AdminController(Context);

            controller.ActionManualActionRequest("requester@example.com", "joe@bloggs.com", "Joe", "Bloggs");

            Context.AccessRequests.Count().Should().Be(2);
            var ar = Context.AccessRequests.Where(x => x.Status == AccessRequest.RequestStatus.Completed).Single();
            
            ar.RequesterEmail.Should().Be("requester@example.com");
            ar.EmailAddress.Should().Be("joe@bloggs.com");
            ar.FirstName.Should().Be("Joe");
            ar.LastName.Should().Be("Bloggs");
            ar.Reason.Should().Be("Manual action (BAT)");
            
            Context.McUsers.Count().Should().Be(2);
            
            var recipient = Context.McUsers.Single(x => x.Email == "joe@bloggs.com");

            recipient.FirstName.Should().Be("Joe");
            recipient.LastName.Should().Be("Bloggs");
            recipient.McOrganisationUsers.Single().OrgId.Should().Be("123");
            recipient.McOrganisationUsers.Single().McOrganisation.Name.Should().Be("TheOrg");
        }

        [Test]
        public void ActionManualAccessRequest_EmptyEmailBadRequest()
        {
            SaveExampleDataToContext();
            var res = new AdminController(Context).ActionManualActionRequest("", "", "Joe", "Bloggs");

            (res as StatusCodeResult).StatusCode.Should().Be(400);
            Context.AccessRequests.Count().Should().Be(1);
        }

        [Test]
        public void ActionManualAccessRequest_NullEmailBadRequest()
        {
            SaveExampleDataToContext();
            var res = new AdminController(Context).ActionManualActionRequest(null, null, "Joe", "Bloggs");

            (res as StatusCodeResult).StatusCode.Should().Be(400);
            Context.AccessRequests.Count().Should().Be(1);
        }

        private void SaveExampleDataToContext()
        {
            var user = new McUser
            {
                FirstName = "RequesterFirstName",
                LastName = "RequesterLastName",
                Email = "requester@example.com",
                McOrganisationUsers = new Collection<McOrganisationUser>()
                {
                    new McOrganisationUser
                    {
                        McOrganisation = new McOrganisation()
                        {
                            OrgId = "123",
                            Name = "TheOrg"
                        }
                    }
                }
            };

            

            Context.AccessRequests.Add(new AccessRequest
            {
                FirstName = "RecipientFirstName",
                LastName = "RecipientLastName",
                EmailAddress = "RECIPIENT@EXAMPLE.com", // check that this gets set to lower case.
                Organisation = "Org",
                Reason = "Reason",
                Requester = user,
                RequesterEmail = user.Email                
            });

            Context.Save();
        }

    }
}