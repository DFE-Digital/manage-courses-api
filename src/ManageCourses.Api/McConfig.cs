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
            var server = _configuration["MANAGE_COURSES_POSTGRESQL_SERVICE_HOST"];
            var port = _configuration["MANAGE_COURSES_POSTGRESQL_SERVICE_PORT"];

            var user = _configuration["PG_USERNAME"];
            var pword = _configuration["PG_PASSWORD"];
            var dbase = _configuration["PG_DATABASE"];

            var sslDefault = "SSL Mode=Prefer;Trust Server Certificate=true";
            var ssl = _configuration["PG_SSL"] ?? sslDefault;

            var connectionString = $"Server={server};Port={port};Database={dbase};User Id={user};Password={pword};{ssl}";
            return connectionString;
        }
    }
}
