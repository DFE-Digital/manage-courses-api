using System.Collections.Generic;
using Notify.Models.Responses;

namespace GovUk.Education.ManageCourses.Api.Services.Email
{
    public interface INotificationClientWrapper
    {
        EmailNotificationResponse SendEmail(string emailAddress, string templateId, Dictionary<string, dynamic> personalisation = null, string clientReference = null, string emailReplyToId = null);
    }
}
