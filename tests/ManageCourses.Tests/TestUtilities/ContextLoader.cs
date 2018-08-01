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
        /// <param name="config"></param>
        /// <param name="dbNameSuffix">Database name suffix to separate different test types into different databases</param>
        public static ManageCoursesDbContext GetDbContext(IConfiguration config, string dbNameSuffix)
        {
            var options = new DbContextOptionsBuilder<ManageCoursesDbContext>()
                .UseNpgsql(Startup.GetConnectionString(config, dbNameSuffix))
                .Options;

            return new ManageCoursesDbContext(options);
        }
    }
}
