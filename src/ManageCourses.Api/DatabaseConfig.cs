using System;
using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.Api
{
    public class DatabaseConfig
    {
        private const string PgDatabaseKey = "PG_DATABASE";
        private const string PgUsernameKey = "PG_USERNAME";
        private const string PgServerKey = "MANAGE_COURSES_POSTGRESQL_SERVICE_HOST";

        private readonly IConfiguration _configuration;

        public DatabaseConfig(IConfiguration configuration)
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

        private string PgServer => _configuration[PgServerKey];
        private string PgPort => _configuration["MANAGE_COURSES_POSTGRESQL_SERVICE_PORT"] ?? "5432";
        private string PgDatabase => _configuration[PgDatabaseKey];
        private string PgUser => _configuration[PgUsernameKey];
        private string PgPassword => _configuration["PG_PASSWORD"];
        private string PgSsl => _configuration["PG_SSL"] ?? "SSL Mode=Prefer;Trust Server Certificate=true";

        /// <summary>
        /// Throws if null or whitespace
        /// </summary>
        /// <param name="key"></param>
        private void ValidateRequired(string key)
        {
            if (string.IsNullOrWhiteSpace(_configuration[key]))
            {
                throw new Exception($"Config value missing: '{key}'");
            }
        }
    }
}
