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

        public UserLogService(IManageCoursesDbContext context)
        {
            _context = context;
        }

        public bool CreateOrUpdateUserLog(string signInUserId, string email) 
        {
            var result = false;
            using (var transaction = ((DbContext)_context).Database.BeginTransaction()) 
            {
                try
                {
                    var user = _context.McUsers.SingleOrDefault(x => x.Email == email);

                    var userLog = _context.UserLogs
                        .Include(x => x.User)
                        .SingleOrDefault(x => x.SignInUserId == signInUserId && x.User.Email == email);

                    var add = userLog == null;
                    if (add)
                    {
                        userLog = new UserLog
                        {
                            FirstLoginDateUtc = DateTime.UtcNow,
                            SignInUserId = signInUserId
                        };
                    }

                    userLog.User = user;
                    userLog.LastLoginDateUtc = DateTime.UtcNow;

                    if (add)
                    {
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
                catch (Exception)
                {
                    transaction.Rollback();
                }

                return result;
            }
        }
    }
}