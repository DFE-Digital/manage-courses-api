using System.Collections.Generic;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.Api.Services
{
    public class WelcomeEmailService: IWelcomeEmailService
    {
        private readonly INotificationClientWrapper _notificationClient;

        private readonly string _templateId;

        public WelcomeEmailService(INotificationClientWrapper noticationClientWrapper, IConfiguration config) : this(noticationClientWrapper, config["email:welcome_template_id"])
        {
        }
        private WelcomeEmailService(INotificationClientWrapper noticationClientWrapper, string templateId)
        {
            _notificationClient = noticationClientWrapper;
            _templateId = templateId;
        }

        public void Send(McUser user)
        {
            var personalisation = new Dictionary<string, dynamic>() { { "first_name", user.FirstName?.Trim() } };
            _notificationClient.SendEmail(user.Email, _templateId, personalisation);
        }
    }
}