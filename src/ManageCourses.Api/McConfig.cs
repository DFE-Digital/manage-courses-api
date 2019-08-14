using System;
using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.Api
{
    public class McConfig
    {
        private readonly IConfiguration _configuration;

        public McConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Build a postgres connection string from configuration data
        /// </summary>
        public string BuildConnectionString()
        {
            var connectionString = $"Server={PgServer};Port={PgPort};Database={PgDatabase};User Id={PgUser};Password={PgPassword};{PgSsl}";
            return connectionString;
        }

        public string SignInUserInfoEndpoint => GetRequired("auth:oidc:userinfo_endpoint");
        public string ApiKey => GetRequired("api:key");
        public string BackendApiKey => GetRequired("manage_courses_backend:key");
        public string EmailApiKey => GetRequired("email:api_key");
        public string EmailTemplateId => GetRequired("email:template_id");
        public string EmailUser => GetRequired("email:user");
        public string SearchAndCompareApiKey => GetRequired("snc:api:key");
        public string SearchAndCompareApiUrl => GetRequired("snc:api:url");
        public string ManageCoursesBackendKey => GetRequired("SETTINGS:MANAGE_BACKEND:SECRET");
        public string ManageCoursesBackendUrl => GetRequired("SETTINGS:MANAGE_BACKEND:BASE_URL");
        private string PgServer => GetRequired("MANAGE_COURSES_POSTGRESQL_SERVICE_HOST");
        private string PgPort => GetRequired("MANAGE_COURSES_POSTGRESQL_SERVICE_PORT");
        private string PgDatabase => GetRequired("PG_DATABASE");
        private string PgUser => _configuration["PG_USERNAME"];
        private string PgPassword => _configuration["PG_PASSWORD"];
        private string PgSsl => _configuration["PG_SSL"];

        private string GetRequired(string key)
        {
            if (string.IsNullOrWhiteSpace(_configuration[key]))
            {
                throw new Exception($"Missing required config key '{key}' (for env vars replace : with double-underscore __)");
            }
            return _configuration[key];
        }
    }
}
