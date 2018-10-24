using System.Collections.Generic;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Services.Email.Model
{
    public class WelcomeEmailModel : IEmailModel
    {
        public string EmailAddress { get; private set; }
        public Dictionary<string, dynamic> Personalisation { get; private set; }

        public WelcomeEmailModel(User user)
        {
            var personalisation = new Dictionary<string, dynamic>() { { "first_name", user.FirstName?.Trim() } };
            this.EmailAddress = user.Email;
            this.Personalisation = personalisation;
        }
    }
}
