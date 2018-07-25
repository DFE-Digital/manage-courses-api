using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Notify.Client;
using Notify.Models.Responses;

namespace GovUk.Education.ManageCourses.Api.Services.Email
{
    /// <summary>
    /// This is a basic wrapper for <see cref="NotificationClient"/>
    /// This class is designed to either use the actual client or to stub it.
    /// </summary>
    public class NotificationClientWrapper : INotificationClientWrapper
    {
        private const string EmailApiKey = "email:api_key";
        private readonly NotificationClient _client;
        private readonly ILogger _logger;
        private readonly bool hasClient;

        public NotificationClientWrapper(IConfiguration config, ILogger<NotificationClientWrapper> logger) : this(config[EmailApiKey], logger)
        {
        }

        private NotificationClientWrapper(string api, ILogger<NotificationClientWrapper> logger)
        {
            _logger = logger;
            if (!string.IsNullOrEmpty(api))
            {
                _client = new NotificationClient(api);
                hasClient = true;
            }
            else
            {
                _logger.LogCritical("Notification Client not configured, emails will *not* be sent. Missing config: {0}", EmailApiKey);
            }
        }


        public EmailNotificationResponse SendEmail(string emailAddress, string templateId, Dictionary<string, dynamic> personalisation = null, string clientReference = null, string emailReplyToId = null)
        {
            EmailNotificationResponse result = null;

            if (hasClient)
            {
                _logger.LogDebug("Sending email using templateId {0}", templateId);
                result = _client.SendEmail(emailAddress, templateId, personalisation, clientReference, emailReplyToId);
            }
            else
            {
                _logger.LogCritical("Email not send. Missing config: {0}", EmailApiKey);
            }

            return result;
        }
    }
}
