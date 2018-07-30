using System;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Services.Email;
using GovUk.Education.ManageCourses.Api.Services.Email.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    public class InviteService
    {
        private readonly IInviteEmailService _inviteEmailService;
        private readonly IManageCoursesDbContext _context;

        public InviteService(IInviteEmailService inviteEmailService,
            IManageCoursesDbContext context)
        {
            _inviteEmailService = inviteEmailService;
            _context = context;
        }
        public void Invite(string email)
        {
            var mcUser = _context.McUsers.ByEmail(email).SingleOrDefault();
            if (mcUser == null)
            {
                throw new Exception("Email not found.");
            }
            var inviteEmailModel = new InviteEmailModel(mcUser);
            _inviteEmailService.Send(inviteEmailModel);
        }
    }
}
