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
            var options = new DbContextOptionsBuilder<ManageCoursesDbContext>()
                .UseNpgsql(
                        Startup.GetConnectionString(config)
                    )
                .Options;

            return new ManageCoursesDbContext(options);
        }
    }
}
