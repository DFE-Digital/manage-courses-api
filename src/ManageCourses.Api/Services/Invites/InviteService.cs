using System.Linq;
using GovUk.Education.ManageCourses.Api.Exceptions;
using GovUk.Education.ManageCourses.Api.Services.Email;
using GovUk.Education.ManageCourses.Api.Services.Email.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;

namespace GovUk.Education.ManageCourses.Api.Services.Invites
{
    public class InviteService : IInviteService
    {
        private readonly IInviteEmailService _inviteEmailService;
        private readonly IManageCoursesDbContext _context;
        private readonly IClock _clock;

        public InviteService(IInviteEmailService inviteEmailService,
            IManageCoursesDbContext context,
            IClock clock)
        {
            _inviteEmailService = inviteEmailService;
            _context = context;
            _clock = clock;
        }

        public void Invite(string email)
        {
            var mcUser = _context.GetUsers(email).SingleOrDefault();
            if (mcUser == null)
            {
                throw new McUserNotFoundException();
            }
            var inviteEmailModel = new InviteEmailModel(mcUser);
            _inviteEmailService.Send(inviteEmailModel);
            if (mcUser.InviteDateUtc == null)
            {
                mcUser.InviteDateUtc = _clock.UtcNow;
                _context.Save();
            }
        }
    }
}
