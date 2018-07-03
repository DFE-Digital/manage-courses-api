using System;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    
    
    [Route("api/[controller]")]
    public class AccessRequestController : Controller
    {
        private readonly IManageCoursesDbContext _context;
        private readonly IEmailService _emailService;

        public AccessRequestController(IManageCoursesDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [Authorize]
        [HttpPost]
        public async Task<StatusCodeResult> Index([FromForm] Api.Model.AccessRequest request) 
        {
            var requesterEmail = this.User.Identity.Name;
            using (var transaction = ((DbContext)_context).Database.BeginTransaction()) 
            {
                try
                {
                    var requester = _context.McUsers
                        .Include(x=>x.McOrganisationUsers)
                        .ThenInclude(x=>x.McOrganisation).Single(x => x.Email == requesterEmail);

                    var requestedIfExists = _context.McUsers
                        .Include(x=>x.McOrganisationUsers)
                        .ThenInclude(x=>x.McOrganisation).SingleOrDefault(x => x.Email == request.EmailAddress);

                    var orgs = requester.McOrganisationUsers.Select(x => x.McOrganisation.Name);
                    var entity = _context.AccessRequests.Add(new Domain.Models.AccessRequest() {
                        RequestDateUtc = System.DateTime.UtcNow,
                        Requester = requester,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        EmailAddress = request.EmailAddress,
                        Organisation = request.Organisation,
                        Reason = request.Reason,
                        Status = Domain.Models.AccessRequest.RequestStatus.Requested
                    }); 
                    _context.Save();

                    string requestedBody;
                    bool requestedAlreadyExistsAsUser = requestedIfExists != null;
                    if (requestedAlreadyExistsAsUser) 
                    {                        
                        requestedBody = 
                            $"The recipient is already on the system as:\n"+
                            $"First name: {requestedIfExists.FirstName}\n"+
                            $"Last name: {requestedIfExists.LastName}\n"+
                            $"With organisations:\n"+
                            string.Join("\n", requestedIfExists.McOrganisationUsers.Select(x => $"- {x.McOrganisation.Name}"))
                            +"\n\n";
                    }
                    else
                    {
                        requestedBody = "The recipient is not yet on the system.\n\n";
                    } 

                    var emailBody = $"{requesterEmail} requested access for {request.FirstName} {request.LastName}\n\n" +
                        
                        $"Request ID: {entity.Entity.Id}\n\n"+

                        $"REQUESTER\n"+
                        $"First name: {requester.FirstName}\n"+
                        $"Last name: {requester.LastName}\n"+
                        $"Email name: {requester.Email}\n\n"+

                        $"REQUESTED\n" +
                        $"First name: {request.FirstName}\n" +
                        $"Last name: {request.LastName}\n" +
                        $"Email address: {request.EmailAddress}\n" +
                        $"Organisation address: {request.Organisation}\n" +
                        $"Reason: {request.Reason}\n\n" +

                        requestedBody +

                        $"The recipient would gain access to the following organisation(s):\n" +
                        string.Join("\n", orgs.Select(x => $"- {x}\n").ToArray());

                    _emailService.SendEmailToSupport($"New access request from {requesterEmail}", emailBody);
                    
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return StatusCode(500);
                }
            
            
                return StatusCode(200);
            }
        }
    }
}