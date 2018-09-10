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
        public async Task<McUser> GetAndUpdateUserAsync(JsonUserDetails userDetails)
        {
            var mcUser = await _context.McUsers.SingleOrDefaultAsync(u => u.SignInUserId == userDetails.Subject);
            if (mcUser == null)
            {
                // fall back to email address for users where we don't yet know their sign-in id
                mcUser = await _context.GetMcUsers(userDetails.Email).SingleOrDefaultAsync();
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
            UpdateMcUserFromSignIn(mcUser, userDetails);

            _context.Save();
            return mcUser;
        }

        /// <inheritdoc />
        public Task LoggedInAsync(McUser user)
        {
            user.LastLoginDateUtc = _clock.UtcNow;
            if (user.FirstLoginDateUtc == null)
            {
                user.FirstLoginDateUtc = _clock.UtcNow;
            }
            SendWelcomeEmail(user);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task CacheTokenAsync(string accessToken, McUser mcUser)
        {
            _context.McSessions.Add(new McSession
            {
                AccessToken = accessToken,
                McUser = mcUser,
                CreatedUtc = _clock.UtcNow
            });

            _context.Save();
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task<McUser> GetFromCacheAsync(string accessToken)
        {
            var dateCutoff = _clock.UtcNow.AddMinutes(-30);
            var session = await _context.McSessions
                .Include(x => x.McUser)
                .FirstOrDefaultAsync(x => x.AccessToken == accessToken && x.CreatedUtc > dateCutoff);
            /*  ^ There is an edge case where more than one record for a valid session 
                could be added, e.g. when clock skew occurs between two authentications. 
                Any of these overlapping records would qualify and the redundancy is quite harmless. 
                So we use First, not Single, here. */

            if (session == null)
            {
                return null;
            }

            return session.McUser;
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
    }
}
