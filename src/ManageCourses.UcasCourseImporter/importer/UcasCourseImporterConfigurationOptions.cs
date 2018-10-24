using System;
using System.Globalization;

using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.UcasCourseImporter
{
    public class UcasCourseImporterConfigurationOptions
    {
        private readonly IConfiguration _config;
 
        public UcasCourseImporterConfigurationOptions(IConfiguration config)
        {
            _config = config;
        }

        public void Validate()
        {
            if(string.IsNullOrWhiteSpace(this.AzureUrl))
            {
                var name = nameof(this.AzureUrl);
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "The '{0}' option must be provided.", name), name);
            }

            if(string.IsNullOrWhiteSpace(this.AzureSignature))
            {
                var name = nameof(this.AzureSignature);
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "The '{0}' option must be provided.", name), name);
            }

            if(string.IsNullOrWhiteSpace(this.ManageApiUrl))
            {
                var name = nameof(this.ManageApiUrl);
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "The '{0}' option must be provided.", name), name);
            }

            if(string.IsNullOrWhiteSpace(this.ManageApiKey)) 
            {
                var name = nameof(this.ManageApiKey);
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "The '{0}' option must be provided.", name), name);
            }
        }

        public string AzureUrl => _config["azure_url"];

        public string AzureSignature => _config["azure_signature"];

        public string ManageApiUrl => _config["manage_api_url"];

        public string ManageApiKey => _config["manage_api_key"];

    }
}