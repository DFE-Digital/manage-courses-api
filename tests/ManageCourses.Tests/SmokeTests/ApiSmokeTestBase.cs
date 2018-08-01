using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Tests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.SmokeTests
{
    public class ApiSmokeTestBase
    {
        protected ApiLocalWebHost localWebHost = null;

        protected IConfiguration config = null;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {

            config = TestConfigBuilder.BuildTestConfig();

            var context = GetContext();

            context.Database.EnsureDeleted();
            context.Database.Migrate();

            localWebHost = new ApiLocalWebHost(config).Launch();
        }

        private ManageCoursesDbContext GetContext()
        {
            var context = ContextLoader.GetDbContext(config, "smoke");
            return context;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (localWebHost != null)
            {
                localWebHost.Stop();
            }

            var context = GetContext();
            context.Database.EnsureDeleted();
        }
    }
}