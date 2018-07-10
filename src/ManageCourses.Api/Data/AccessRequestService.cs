using System;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using Microsoft.EntityFrameworkCore;

namespace GovUk.Education.ManageCourses.Api.Data
{
    public class AccessRequestService : IAccessRequestService
    {
        
        private readonly IManageCoursesDbContext _context;
        private readonly IAccessRequestEmailService _emailService;

        
        public AccessRequestService(IManageCoursesDbContext context, IAccessRequestEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public void LogAccessRequest(AccessRequest request, string requesterEmail)
        {
            using (var transaction = ((DbContext)_context).Database.BeginTransaction()) 
            {
                try
                {
                    var requester = _context.McUsers
                        .Include(x=>x.McOrganisationUsers)
                        .ThenInclude(x=>x.McOrganisation)
                        .Single(x => String.Equals(x.Email, requesterEmail, StringComparison.InvariantCultureIgnoreCase));

                    var requestedIfExists = _context.McUsers
                        .Include(x=>x.McOrganisationUsers)
                        .ThenInclude(x=>x.McOrganisation).SingleOrDefault(x =>String.Equals(x.Email, request.EmailAddress, StringComparison.InvariantCultureIgnoreCase));

                    var orgs = requester.McOrganisationUsers.Select(x => x.McOrganisation.Name);
                    var entity = _context.AccessRequests.Add(new Domain.Models.AccessRequest() {
                        RequestDateUtc = System.DateTime.UtcNow,
                        Requester = requester,
                        RequesterEmail = requester.Email,
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
                    throw;
                }
            }
        }        
    }
}
