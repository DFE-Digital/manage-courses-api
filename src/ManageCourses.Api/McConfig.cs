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
            var server = PgServer;
            var port = PgPort;

            var user = PgUser;
            var pword = PgPassword;
            var dbase = PgDatabase;

            var sslDefault = "SSL Mode=Prefer;Trust Server Certificate=true";
            var ssl = PgSsl ?? sslDefault;

            var connectionString = $"Server={server};Port={port};Database={dbase};User Id={user};Password={pword};{ssl}";
            return connectionString;
        }

        private string PgServer => _configuration["MANAGE_COURSES_POSTGRESQL_SERVICE_HOST"];
        private string PgPort => _configuration["MANAGE_COURSES_POSTGRESQL_SERVICE_PORT"];
        private string PgDatabase => _configuration["PG_DATABASE"];
        private string PgUser => _configuration["PG_USERNAME"];
        private string PgPassword => _configuration["PG_PASSWORD"];
        private string PgSsl => _configuration["PG_SSL"];
    }
}
