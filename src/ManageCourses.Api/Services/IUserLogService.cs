using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Services
{
    public interface IUserLogService
    {
        bool CreateOrUpdateUserLog(string signInUserId, McUser user);
    }
}
