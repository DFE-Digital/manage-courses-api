namespace GovUk.Education.ManageCourses.Api.Services
{
    public interface IUserLogService
    {
        bool CreateOrUpdateUserLog(string signInUserId, string email);
    }
}
