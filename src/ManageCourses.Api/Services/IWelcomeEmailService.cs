using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Services
{
    public interface IWelcomeEmailService
    {
        void Send(McUser user);
    }
}
