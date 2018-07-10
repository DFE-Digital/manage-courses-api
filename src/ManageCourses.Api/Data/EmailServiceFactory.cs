using System;

namespace GovUk.Education.ManageCourses.Api.Data
{
    public class EmailServiceFactory
    {
        private readonly string _apiKey;

        public EmailServiceFactory(string apiKey) 
        {
            _apiKey = apiKey;
        }

        public IAccessRequestEmailService MakeAccessRequestEmailService(string templateId, string user) 
        {
            if(!String.IsNullOrWhiteSpace(_apiKey))
            {
                return new AccessRequestEmailService(_apiKey, templateId, user);
            }
            else
            {
                // for dev and test environments that don't want to spam our support email
                return new NullAccessRequestEmailService();
            }
        }
    }
}