using System;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GovUk.Education.ManageCourses.Api.Model;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    
    
    [Route("api/[controller]")]
    public class AccessRequestController : Controller
    {
        private readonly IManageCoursesDbContext _context;
        private readonly IAccessRequestEmailService _emailService;

        public AccessRequestController(IManageCoursesDbContext context, IAccessRequestEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [Authorize]
        [HttpPost]
        public async Task<StatusCodeResult> Index([FromBody] AccessRequest request) 
        {
            var requesterEmail = this.User.Identity.Name;
            using (var transaction = ((DbContext)_context).Database.BeginTransaction()) 
            {
                try
                {
                    var requester = _context.McUsers
                        .Include(x=>x.McOrganisationUsers)
                        .ThenInclude(x=>x.McOrganisation).ByEmail(requesterEmail).SingleOrDefault();

                    var requestedIfExists = _context.McUsers
                        .Include(x=>x.McOrganisationUsers)
                        .ThenInclude(x=>x.McOrganisation).ByEmail(request.EmailAddress).SingleOrDefault();

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

                    _emailService.SendAccessRequestEmailToSupport(entity.Entity, requester, requestedIfExists);
                                        
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return StatusCode(500);
                }
            
            
                return StatusCode(200);
            }
        }
    }
}