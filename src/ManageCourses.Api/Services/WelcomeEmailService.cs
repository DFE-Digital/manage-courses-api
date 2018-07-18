using System.Collections.Generic;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.Api.Services
{
    public class WelcomeEmailService: IWelcomeEmailService
    {
        private readonly ITemplateEmailService _emailService;

        public WelcomeEmailService(ITemplateEmailService emailService)
        {
            _emailService = emailService;
        }

        public void Send(McUser user)
        {
            var personalisation = new Dictionary<string, dynamic>() { { "first_name", user.FirstName?.Trim() } };
            _emailService.Send(user.Email, personalisation);
        }
    }
}
