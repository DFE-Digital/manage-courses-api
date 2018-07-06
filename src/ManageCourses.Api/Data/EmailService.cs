
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Domain.Models;
using Notify.Client;

namespace GovUk.Education.ManageCourses.Api.Data
{
    public class EmailService : IEmailService
    {
        private NotificationClient _notificationClient;

        private string _user;

        private string _templateId;

        // This class only supports the RFC-compliant port 587
        private const int SupportedSmtpPort = 587;

        public EmailService(string apiKey, string templateId, string user)
        {
            _notificationClient = String.IsNullOrWhiteSpace(apiKey) ? null : new NotificationClient(apiKey);
            _templateId = templateId;
            _user = user;
        }

        public void SendAccessRequestEmailToSupport(AccessRequest accessRequest, McUser requester, McUser requestedOrNull)
        {
            var templateValues = new Dictionary<string, dynamic>() {
                {"request_id", accessRequest.Id},

                {"requester_firstname", requester.FirstName},
                {"requester_lastname", requester.LastName},
                {"requester_email", requester.Email},
                {"requester_existingorgs", String.Join(", ", requester.McOrganisationUsers.Select(x => x.McOrganisation.Name))},

                {"requested_firstname", accessRequest.FirstName},
                {"requested_lastname", accessRequest.LastName},
                {"requested_email", accessRequest.EmailAddress},
                {"requested_organisation", accessRequest.Organisation},
                {"requested_reason", accessRequest.Reason},

                {"requested_existingorgs", requestedOrNull == null ? "" : String.Join(", ", requestedOrNull.McOrganisationUsers.Select(x => x.McOrganisation.Name))}

            };

            _notificationClient.SendEmail(_user,  _templateId, templateValues);
        }

        public bool ShouldBeAbleToSend() 
        {
            return _notificationClient != null;
        }
    }
}