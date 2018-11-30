using System;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using Microsoft.EntityFrameworkCore;

namespace GovUk.Education.ManageCourses.Api.Services.AccessRequests
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

        public DbContext RawContext => (DbContext)_context;

        public void LogAccessRequest(AccessRequest request, string requesterEmail)
        {
            // Strategy is required for compatibility with EnableRetryOnFailure in Startup. Ref https://docs.microsoft.com/en-gb/azure/architecture/best-practices/retry-service-specific#sql-database-using-entity-framework-core
            var strategy = RawContext.Database.CreateExecutionStrategy();
            strategy.Execute(() =>
            {
                using (var transaction = RawContext.Database.BeginTransaction())
                {
                    try
                    {
                        var requester = _context
                            .GetUsers(requesterEmail)
                            .Include(x => x.OrganisationUsers)
                            .ThenInclude(x => x.Organisation)
                            .Single();

                        var requestedIfExists = _context
                            .GetUsers(request.EmailAddress)
                            .Include(x => x.OrganisationUsers)
                            .ThenInclude(x => x.Organisation)
                            .SingleOrDefault();

                        var entity = _context.AccessRequests.Add(new Domain.Models.AccessRequest()
                        {
                            RequestDateUtc = DateTime.UtcNow,
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
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            });
        }
    }
}
