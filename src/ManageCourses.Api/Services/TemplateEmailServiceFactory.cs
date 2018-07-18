using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace GovUk.Education.ManageCourses.Api.Services
{
    public class TemplateEmailServiceFactory : ITemplateEmailServiceFactory
    {
        private readonly INotificationClientWrapper _notificationClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public TemplateEmailServiceFactory(INotificationClientWrapper notificationClient, IConfiguration configuration, ILogger<ITemplateEmailServiceFactory> logger)
        {
            _notificationClient = notificationClient;
            _configuration = configuration;
            _logger = logger;
        }

        public ITemplateEmailService Build(string templateKey)
        {
            var templateId = _configuration[templateKey];
            var msg = "Using templateKey : {0}, templateId : {1}";
            _logger.LogInformation(msg, templateKey, templateId);

            return new TemplateEmailService(_notificationClient, templateId);
        }
    }
}
