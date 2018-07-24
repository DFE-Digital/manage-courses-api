using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Api.Services.Email.Model
{
    /// <summary>
    /// The content for the email.
    /// </summary>
    public interface IEmailModel
    {
        string EmailAddress { get; }
        Dictionary<string, dynamic> Personalisation { get; }
    }
}
