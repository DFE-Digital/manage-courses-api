using System;
using System.Linq;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GovUk.Education.ManageCourses.Api.Services
{
    public class UserLogService : IUserLogService
    {
        private readonly IManageCoursesDbContext _context;
        private readonly IWelcomeEmailService _welcomeEmailService;

        public UserLogService(IManageCoursesDbContext context, IWelcomeEmailService welcomeEmailService)
        {
            _context = context;
            _welcomeEmailService = welcomeEmailService;
        }

        public bool CreateOrUpdateUserLog(string signInUserId, McUser user)
        {
            var result = false;
            using (var transaction = ((DbContext)_context).Database.BeginTransaction())
            {
                try
                {
                    var userLog = GetUserLog(signInUserId, user);
                    var add = userLog.Id < 1;

                    if (add)
                    {
                        _welcomeEmailService.Send(user);
                        _context.UserLogs.Add(userLog);
                    }
                    else
                    {
                        _context.UserLogs.Update(userLog);
                    }

                    _context.Save();
                    transaction.Commit();
                    result = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }

                return result;
            }
        }

        private UserLog Create(string signInUserId, DateTime now)
        {
            return new UserLog
            {
                FirstLoginDateUtc = now,
                SignInUserId = signInUserId,
                WelcomeEmailDateUtc = now
            };
        }

        private UserLog GetUserLog(string signInUserId, McUser user)
        {
            var userLog = _context.UserLogs
                .SingleOrDefault(x => x.SignInUserId == signInUserId);

            var add = userLog == null;
            var now = DateTime.UtcNow;
            if (add)
            {
                userLog = Create(signInUserId, now);
            }

            userLog.User = user;
            userLog.UserEmail = user.Email;
            userLog.LastLoginDateUtc = now;

            return userLog;
        }
    }
}