using GovUk.Education.ManageCourses.Api.Services.Email.Model;
using GovUk.Education.ManageCourses.Api.Services.Email.Config;

namespace GovUk.Education.ManageCourses.Api.Services.Email
{
    public class WelcomeEmailService : TemplateEmailService<WelcomeEmailModel>, IWelcomeEmailService
    {
        public WelcomeEmailService(INotificationClientWrapper noticationClientWrapper, IWelcomeTemplateEmailConfig config)
            : base(noticationClientWrapper, config)
        {
        }
    }
}
