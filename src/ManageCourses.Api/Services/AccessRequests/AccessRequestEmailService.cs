using System;
using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Domain.Models;
using Notify.Client;

namespace GovUk.Education.ManageCourses.Api.Services.AccessRequests
{
    public class AccessRequestEmailService : IAccessRequestEmailService
    {
        private readonly NotificationClient _notificationClient;

        private readonly string _user;

        private readonly string _templateId;

        // This class only supports the RFC-compliant port 587
        private const int SupportedSmtpPort = 587;

        public AccessRequestEmailService(string apiKey, string templateId, string user)
        {
            if (String.IsNullOrWhiteSpace(apiKey)) 
            {
                throw new ArgumentNullException(nameof(apiKey));
            }
            if (String.IsNullOrWhiteSpace(templateId)) 
            {
                throw new ArgumentNullException(nameof(templateId));
            }
            if (String.IsNullOrWhiteSpace(user)) 
            {
                throw new ArgumentNullException(nameof(user));
            }

            _notificationClient = new NotificationClient(apiKey);
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

    }
}