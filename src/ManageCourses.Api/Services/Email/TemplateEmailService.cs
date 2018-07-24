using GovUk.Education.ManageCourses.Api.Services.Email.Model;
using GovUk.Education.ManageCourses.Api.Services.Email.Config;

namespace GovUk.Education.ManageCourses.Api.Services.Email
{
    public class TemplateEmailService<TEmailModel> : ITemplateEmailService<TEmailModel> where TEmailModel : class, IEmailModel
    {
        private readonly INotificationClientWrapper _notificationClient;

        private readonly ITemplateEmailConfig<TEmailModel> _config;

        public TemplateEmailService(INotificationClientWrapper noticationClientWrapper, ITemplateEmailConfig<TEmailModel> config)
        {
            _notificationClient = noticationClientWrapper;
            _config = config;
        }

        public void Send(TEmailModel model)
        {
            _notificationClient.SendEmail(model.EmailAddress, _config.TemplateId, model.Personalisation);
        }
    }
}

