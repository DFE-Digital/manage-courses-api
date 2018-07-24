using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Api.Services.Email.Model
{
    public interface IEmailModel
    {
        string EmailAddress { get; }
        Dictionary<string, dynamic> Personalisation { get; }
    }
}
