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
        public static ManageCoursesDbContext GetDbContext(IConfiguration config)
        {
            var mcConfig = new McConfig(config);
            var connectionString = mcConfig.BuildConnectionString();

            var options = new DbContextOptionsBuilder<ManageCoursesDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            return new ManageCoursesDbContext(options);
        }
    }
}
