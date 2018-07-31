using System.IO;
using GovUk.Education.ManageCourses.Api;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.Tests.TestUtilities
{
    internal class ContextLoader
    {
        public const string IntegrationTestJson = "integration-tests.json";

        /// <summary>
        /// Configures and returns a manage-courses dbContext
        /// </summary>
        /// <param name="configFilename">name of json file to use for config data</param>
        public static ManageCoursesDbContext GetDbContext(string configFilename)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(configFilename)
                .Build();

            var options = new DbContextOptionsBuilder<ManageCoursesDbContext>()
                .UseNpgsql(Startup.GetConnectionString(config))
                .Options;

            return new ManageCoursesDbContext(options);
        }
    }
}
