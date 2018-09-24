using System;
using System.Collections.ObjectModel;
using System.Linq;
using GovUk.Education.ManageCourses.Api.ActionFilters;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static GovUk.Education.ManageCourses.Domain.Models.AccessRequest;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    [Route("api/admin")]
    public class AdminController : Controller
    {
        private readonly IManageCoursesDbContext _context;

        public AdminController(IManageCoursesDbContext context)
        {
            _context = context;
        }

        [HttpPost("access-request")]
        [ApiTokenAuth]
        [ExemptFromAcceptTerms]
        public IActionResult ActionAccessRequest(int accessRequestId)
        {
            // Include instructs EF to also retrieve data that's connected with a foreign key
            // With ThenInclude you can also retrieve relations of relations
            var request = _context.AccessRequests
                .Include(accessRequest => accessRequest.Requester)
                .ThenInclude(requester => requester.McOrganisationUsers)
                .SingleOrDefault(x => x.Id == accessRequestId);

            if (request == null)
            {
                return NotFound();
            }

            var requestedEmail = request.EmailAddress.ToLower();

            // if a user gets recreated, this breaks the FK relation to mcUser, so we try to 
            // recover by finding the new user by email
            var requesterUser = request.Requester ?? _context.McUsers.Include(x => x.McOrganisationUsers).SingleOrDefault(x => x.Email == request.RequesterEmail);

            if (requesterUser == null)
            {
                return NotFound();
            }

            UpdateAccessRights(requestedEmail, request.FirstName, request.LastName, requesterUser);

            request.Status = RequestStatus.Completed;

            _context.Save();
            return Ok();
        }

        [HttpPost("manual-access-request")]
        [ApiTokenAuth]
        [ExemptFromAcceptTerms]
        public IActionResult ActionManualActionRequest(string requesterEmail, string targetEmail, string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(requesterEmail) || string.IsNullOrWhiteSpace(targetEmail))
            {
                return BadRequest();
            }

            requesterEmail = requesterEmail.ToLower();
            var requesterUser = _context.McUsers
                .Include(x=>x.McOrganisationUsers)
                .SingleOrDefault(x => x.Email == requesterEmail);

            if (requesterUser == null)
            {
                return NotFound();
            }

            _context.AccessRequests.Add(new AccessRequest {
                RequestDateUtc = DateTime.UtcNow,
                RequesterEmail = requesterEmail,
                Requester = requesterUser,
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = targetEmail,
                Reason = "Manual action (BAT)",
                Status = RequestStatus.Completed
            });

            UpdateAccessRights(targetEmail, firstName, lastName, requesterUser);
            
            _context.Save();
            return Ok();
        }

        private void UpdateAccessRights(string requestedEmail, string firstName, string lastName, McUser requesterUser)
        {
            var requesterEmail = requesterUser.Email.ToLower();

            var existingTargetUser = _context.McUsers
                            .Include(x => x.McOrganisationUsers)
                            .SingleOrDefault(x => x.Email == requestedEmail); // throws if email is ambiguous

            if (existingTargetUser == null)
            {
                // insert
                _context.McUsers.Add(new McUser
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = requestedEmail,
                    InviteDateUtc = DateTime.Now,
                    McOrganisationUsers = new Collection<McOrganisationUser>(
                            requesterUser.McOrganisationUsers.Select(x => new McOrganisationUser
                            {
                                OrgId = x.OrgId
                            }).ToList())
                });
            }
            else
            {
                // update
                foreach (var organisationUser in requesterUser.McOrganisationUsers)
                {
                    if (!existingTargetUser.McOrganisationUsers.Any(x => x.OrgId == organisationUser.OrgId))
                    {
                        existingTargetUser.McOrganisationUsers.Add(new McOrganisationUser
                        {
                            Email = existingTargetUser.Email,
                            OrgId = organisationUser.OrgId
                        });
                    }
                }
            }
        }
    }
}