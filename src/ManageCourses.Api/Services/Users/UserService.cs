using System;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Exceptions;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Api.Services.Email;
using GovUk.Education.ManageCourses.Api.Services.Email.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GovUk.Education.ManageCourses.Api.Services.Users
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
        public async Task UserSignedInAsync(string accessToken, JsonUserDetails userDetails)
        {
            var mcUser = await _context.McUsers.SingleOrDefaultAsync(u => u.SignInUserId == userDetails.Subject);
            if (mcUser == null)
            {
                // fall back to email address for users where we don't yet know their sign-in id
                mcUser = await _context.McUsers.ByEmail(userDetails.Email).SingleOrDefaultAsync();
                if (mcUser != null)
                {
                    // record the sign-in id and use that in future
                    mcUser.SignInUserId = userDetails.Subject;
                }
            }
            if (mcUser == null)
            {
                throw new McUserNotFoundException();
            }
            LogLogin(mcUser);
            UpdateMcUserFromSignIn(mcUser, userDetails);

            if (!_context.McSessions.Any(x => x.AccessToken == accessToken && x.CreatedUtc > _clock.UtcNow.AddMinutes(-30)))
            {
                _context.McSessions.Add(new McSession{
                    AccessToken = accessToken,
                    McUser = mcUser,
                    Email = mcUser.Email,
                    Subject = userDetails.Subject,
                    CreatedUtc = _clock.UtcNow
                });
            }

            _context.Save();
            SendWelcomeEmail(mcUser);
        }

        private void SendWelcomeEmail(McUser user)
        {
            if (user.WelcomeEmailDateUtc == null)
            {
                var welcomedata = new WelcomeEmailModel(user);
                _welcomeEmailService.Send(welcomedata);
                user.WelcomeEmailDateUtc = _clock.UtcNow;
            }
            _context.Save();
        }

        private static void UpdateMcUserFromSignIn(McUser user, JsonUserDetails userDetails)
        {
            //user.Email = userDetails.Email; // todo: update email address from sign-in. blocked by use of email as a foreign-key
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
