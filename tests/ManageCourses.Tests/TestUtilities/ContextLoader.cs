using System;
using System.Collections.Generic;
using GovUk.Education.ManageCourses.Api;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.Tests.TestUtilities
{
    internal class ContextLoader
    {
        /// <summary>
        /// Configures and returns a manage-courses dbContext
        /// </summary>
        public static ManageCoursesDbContext GetDbContext(IConfiguration config, bool enableRetryOnFailure = true)
        {
            var mcConfig = new McConfig(config);
            var connectionString = mcConfig.BuildConnectionString();

            const int maxRetryCount = 1; // don't actually allow retry for tests
            const int maxRetryDelaySeconds = 1;

            var postgresErrorCodesToConsiderTransient = new List<string>(); // ref: https://github.com/npgsql/Npgsql.EntityFrameworkCore.PostgreSQL/blob/16c8d07368cb92e10010b646098b562ecd5815d6/src/EFCore.PG/NpgsqlRetryingExecutionStrategy.cs#L99

            // Configured to be similar to context setup in real app
            // Importantly the retry is enabled as that is what the production code uses and that is incompatible with the normal transaction pattern.
            // This will allow us to catch any re-introduction of following error before the code ships: "The configured execution strategy 'NpgsqlRetryingExecutionStrategy' does not support user initiated transactions. Use the execution strategy returned by 'DbContext.Database.CreateExecutionStrategy()' to execute all the operations in the transaction as a retriable unit."
            var options = new DbContextOptionsBuilder<ManageCoursesDbContext>()
                .UseNpgsql(connectionString, b =>
                            {
                                b.MigrationsAssembly((typeof(ManageCoursesDbContext).Assembly).ToString());
                                if (enableRetryOnFailure)
                                {
                                    b.EnableRetryOnFailure(maxRetryCount, TimeSpan.FromSeconds(maxRetryDelaySeconds), postgresErrorCodesToConsiderTransient);
                                }
                            })
                .Options;

            return new ManageCoursesDbContext(options);
        }
    }
}
