using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.Api
{
    public class McConfig
    {
        private const string ConfigKeyForPgDatabase = "PG_DATABASE";
        private const string ConfigKeyForPgUsername = "PG_USERNAME";
        private const string ConfigKeyForPgServer = "MANAGE_COURSES_POSTGRESQL_SERVICE_HOST";
        private const string ConfigKeyForSignInUserInfoEndpoint = "auth:oidc:userinfo_endpoint";
        private const string ConfigKeyForApiKey = "api:key";
        private const string ConfigKeyForBackendApiKey = "manage_courses_backend:key";
        private const string ConfigKeyForEmailApiKey = "email:api_key";
        private const string ConfigKeyForEmailTemplateId = "email:template_id";
        private const string ConfigKeyForEmailUser = "email:user";
        private const string ConfigKeyForSearchAndCompareApiKey = "snc:api:key";
        private const string ConfigKeyForSearchAndCompareApiUrl = "snc:api:url";

        private readonly IConfiguration _configuration;

        public McConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Check all required config is present.
        /// Intended to be run early to catch config problems at startup instead of during normal operation.
        /// </summary>
        public void Validate()
        {
            var requiredKeys = new List<string>
            {
                ConfigKeyForPgServer,
                ConfigKeyForPgDatabase,
                ConfigKeyForPgUsername,
                ConfigKeyForSignInUserInfoEndpoint,
                ConfigKeyForApiKey,
                ConfigKeyForBackendApiKey,
                ConfigKeyForSearchAndCompareApiKey,
                ConfigKeyForSearchAndCompareApiUrl,
            };

            var emailKeys = new List<string>
            {
                ConfigKeyForEmailApiKey,
                ConfigKeyForEmailTemplateId,
                ConfigKeyForEmailUser,
            };

            var invalidKeys = requiredKeys.Where(k => string.IsNullOrWhiteSpace(_configuration[k])).ToList();

            // Email config is optional, if email config missing emails will not be sent and instead a warning emitted by the app
            if (emailKeys.Any(ek => !string.IsNullOrWhiteSpace(_configuration[ek])))
            {
                // add missing keys from email set to error list
                invalidKeys.AddRange(emailKeys.Where(ek => string.IsNullOrWhiteSpace(_configuration[ek])));
            }

            if (invalidKeys.Any())
            {
                throw new Exception(
                    $"Missing config keys: {string.Join(", ", invalidKeys)}\n- note that email config is optional but if any email key is set then all email keys must be set.");
            }
        }

        /// <summary>
        /// Build a postgres connection string from configuration data
        /// </summary>
        public string BuildConnectionString()
        {
            // Validate();
            var connectionString = $"Server={PgServer};Port={PgPort};Database={PgDatabase};User Id={PgUser};Password={PgPassword};{PgSsl}";
            return connectionString;
        }

        public string SignInUserInfoEndpoint => _configuration[ConfigKeyForSignInUserInfoEndpoint];
        public string ApiKey => _configuration[ConfigKeyForApiKey];
        public string BackendApiKey => _configuration[ConfigKeyForBackendApiKey];
        public string EmailApiKey => _configuration[ConfigKeyForEmailApiKey];
        public string EmailTemplateId => _configuration[ConfigKeyForEmailTemplateId];
        public string EmailUser => _configuration[ConfigKeyForEmailUser];
        public string SearchAndCompareApiKey => _configuration[ConfigKeyForSearchAndCompareApiKey];
        public string SearchAndCompareApiUrl => _configuration[ConfigKeyForSearchAndCompareApiUrl];

        private string PgServer => _configuration[ConfigKeyForPgServer];
        private string PgPort => _configuration["MANAGE_COURSES_POSTGRESQL_SERVICE_PORT"] ?? "5432";
        private string PgDatabase => _configuration[ConfigKeyForPgDatabase];
        private string PgUser => _configuration[ConfigKeyForPgUsername];
        private string PgPassword => _configuration["PG_PASSWORD"];
        private string PgSsl => _configuration["PG_SSL"] ?? "SSL Mode=Prefer;Trust Server Certificate=true";
    }
}
