using GovUk.Education.ManageCourses.Api.Services.Email.Model;

namespace GovUk.Education.ManageCourses.Api.Services.Email
{
    public interface IWelcomeEmailService : ITemplateEmailService<WelcomeEmailModel>
    {
    }
}
