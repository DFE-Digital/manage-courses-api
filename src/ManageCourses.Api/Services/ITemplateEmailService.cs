using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Api.Services
{
    public interface ITemplateEmailService
    {
        void Send(string email, Dictionary<string, dynamic> personalisation);
    }
}
