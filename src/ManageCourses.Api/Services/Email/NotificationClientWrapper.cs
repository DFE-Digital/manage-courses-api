using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Notify.Client;
using Microsoft.Extensions.Logging;
using Notify.Models.Responses;
using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.Api.Services.Email
{
    /// <summary>
    /// This is a basic wrapper for <see cref="NotificationClient"/>
    /// This class is designed to either use the actual client or to stub it.
    /// </summary>
    public class NotificationClientWrapper : INotificationClientWrapper
    {
        private readonly NotificationClient _client = null;
        private readonly ILogger _logger = null;
        private readonly bool hasClient = false;

        public NotificationClientWrapper(IConfiguration config, ILogger<NotificationClientWrapper> logger) : this(config["email:api_key"], logger)
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

            var clientType = hasClient ? "LIVE" : "Stub";

            _logger.LogInformation("Using {0} Notification Client", clientType);
        }


        public EmailNotificationResponse SendEmail(string emailAddress, string templateId, Dictionary<string, dynamic> personalisation = null, string clientReference = null, string emailReplyToId = null)
        {
            EmailNotificationResponse result = null;

            if (hasClient) {
                _logger.LogDebug("Sending email using templateId {0}", templateId);
                result = _client.SendEmail(emailAddress, templateId, personalisation, clientReference, emailReplyToId);
            }
            else
            {
                _logger.LogCritical("No email is sent");
            }

            return result;
        }
    }
}
