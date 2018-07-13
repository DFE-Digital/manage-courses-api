using System;
using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Domain.Models;
using Notify.Client;

namespace GovUk.Education.ManageCourses.Api.Services
{
    public class WelcomeEmailService: IWelcomeEmailService
    {
        private readonly NotificationClient _notificationClient;

        private readonly string _templateId;

        public WelcomeEmailService(string apiKey, string templateId)
        {
            if (String.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey));
            }

            if (String.IsNullOrWhiteSpace(templateId))
            {
                throw new ArgumentNullException(nameof(templateId));
            }

            _notificationClient = new NotificationClient(apiKey);
            _templateId = templateId;
        }

        public void Send(McUser user)
        {
            var personalisation = new Dictionary<string, dynamic>() { { "first_name", user.FirstName?.Trim() } };
            _notificationClient.SendEmail(user.Email, _templateId, personalisation);
        }
    }
}