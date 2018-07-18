using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Api.Services
{

    public class TemplateEmailService : ITemplateEmailService
    {
        private readonly string _templateId;
        private readonly INotificationClientWrapper _notificationClient;

        public TemplateEmailService(INotificationClientWrapper noticationClientWrapper, string templateId)
        {
            _notificationClient = noticationClientWrapper;
            _templateId = templateId;
        }

        public void Send(string email, Dictionary<string, dynamic> personalisation)
        {
            _notificationClient.SendEmail(email, _templateId, personalisation);
        }
    }
}