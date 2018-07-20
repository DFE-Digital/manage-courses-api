using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GovUk.Education.ManageCourses.Api.Services
{
    /// <inheritdoc />
    public class UserService : IUserService
    {
        private readonly IManageCoursesDbContext _context;
        private readonly IWelcomeEmailService _welcomeEmailService;
        private readonly IClock _clock;

        /// <inheritdoc />
        public UserService(IManageCoursesDbContext context, IWelcomeEmailService welcomeEmailService, IClock clock)
        {
            _context = context;
            _welcomeEmailService = welcomeEmailService;
            _clock = clock;
        }

        /// <inheritdoc />
        public async Task UserSignedInAsync(JsonUserDetails userDetails)
        {
            var mcUser = await _context.McUsers.ByEmail(userDetails.Email).SingleOrDefaultAsync();
            if (mcUser == null)
            {
                throw new UnknownMcUserException();
            }
            LogLogin(mcUser);
            UpdateMcUserFromSignIn(mcUser, userDetails);
            _context.Save();
            SendWelcomeEmail(mcUser);
        }

        private void SendWelcomeEmail(McUser user)
        {
            if (user.WelcomeEmailDateUtc == null)
            {
                _welcomeEmailService.Send(user);
                user.WelcomeEmailDateUtc = _clock.UtcNow;
            }
            _context.Save();
        }

        private static void UpdateMcUserFromSignIn(McUser user, JsonUserDetails userDetails)
        {
            if (user.SignInUserId == null)
            {
                user.SignInUserId = userDetails.Subject;
            }
            user.Email = userDetails.Email;
            user.FirstName = userDetails.GivenName;
            user.LastName = userDetails.FamilyName;
        }

        private void LogLogin(McUser user)
        {
            user.LastLoginDateUtc = _clock.UtcNow;
            if (user.FirstLoginDateUtc == null)
            {
                user.FirstLoginDateUtc = _clock.UtcNow;
            }
        }
    }
}
