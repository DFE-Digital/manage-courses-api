using System;
using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.Api
{
    public class McConfig
    {
        private const string PgDatabaseKey = "PG_DATABASE";
        private const string PgUsernameKey = "PG_USERNAME";
        private const string PgServerKey = "MANAGE_COURSES_POSTGRESQL_SERVICE_HOST";
        private const string SignInUserInfoEndpointConfigKey = "auth:oidc:userinfo_endpoint";
        private const string ApiKeyConfigKey = "api:key";
        private const string EmailApiKeyConfigKey = "email:api_key";
        private const string EmailTemplateIdConfigKey = "email:template_id";
        private const string EmailUserConfigKey = "email:user";
        private const string SearchAndCompareApiKeyConfigKey = "snc:api:key";
        private const string SearchAndCompareApiUrlConfigKey = "snc:api:url";
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
            ValidateRequired(PgServerKey);
            ValidateRequired(PgDatabaseKey);
            ValidateRequired(PgUsernameKey);
            ValidateRequired(SignInUserInfoEndpointConfigKey);
            ValidateRequired(ApiKeyConfigKey);
            ValidateRequired(EmailApiKeyConfigKey);
            ValidateRequired(EmailTemplateIdConfigKey);
            ValidateRequired(EmailUserConfigKey);
            ValidateRequired(SearchAndCompareApiKeyConfigKey);
            ValidateRequired(SearchAndCompareApiUrlConfigKey);
        }

        /// <summary>
        /// Build a postgres connection string from configuration data
        /// </summary>
        public string BuildConnectionString()
        {
            Validate();
            var connectionString = $"Server={PgServer};Port={PgPort};Database={PgDatabase};User Id={PgUser};Password={PgPassword};{PgSsl}";
            return connectionString;
        }

        public string SignInUserInfoEndpoint => _configuration[SignInUserInfoEndpointConfigKey];
        public string ApiKey => _configuration[ApiKeyConfigKey];
        public string EmailApiKey => _configuration[EmailApiKeyConfigKey];
        public string EmailTemplateId => _configuration[EmailTemplateIdConfigKey];
        public string EmailUser => _configuration[EmailUserConfigKey];
        public string SearchAndCompareApiKey => _configuration[SearchAndCompareApiKeyConfigKey];
        public string SearchAndCompareApiUrl => _configuration[SearchAndCompareApiUrlConfigKey];
        private string PgServer => _configuration[PgServerKey];
        private string PgPort => _configuration["MANAGE_COURSES_POSTGRESQL_SERVICE_PORT"] ?? "5432";
        private string PgDatabase => _configuration[PgDatabaseKey];
        private string PgUser => _configuration[PgUsernameKey];
        private string PgPassword => _configuration["PG_PASSWORD"];
        private string PgSsl => _configuration["PG_SSL"] ?? "SSL Mode=Prefer;Trust Server Certificate=true";

        /// <summary>
        /// Throws if null or whitespace config value for this key is blank or missing.
        /// </summary>
        /// <param name="key">The config key to check</param>
        private void ValidateRequired(string key)
        {
            if (string.IsNullOrWhiteSpace(_configuration[key]))
            {
                throw new Exception($"Config value missing: '{key}'");
            }
        }
    }
}
