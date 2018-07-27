using GovUk.Education.ManageCourses.Api.Services.Email.Model;
using GovUk.Education.ManageCourses.Api.Services.Email.Config;

namespace GovUk.Education.ManageCourses.Api.Services.Email
{
    public class InviteEmailService : TemplateEmailService<InviteEmailModel>, IInviteEmailService
    {
        public InviteEmailService(INotificationClientWrapper noticationClientWrapper, IInviteTemplateEmailConfig config)
            : base(noticationClientWrapper, config)
        {
        }
    }
}
