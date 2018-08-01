using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Tests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.SmokeTests
{
    public abstract class ApiSmokeTestBase
    {
        protected ApiLocalWebHost LocalWebHost;
        protected IConfiguration Config;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {

            Config = TestConfigBuilder.BuildTestConfig();

            var context = GetContext();

            context.Database.EnsureDeleted();
            context.Database.Migrate();

            LocalWebHost = new ApiLocalWebHost(Config).Launch();
        }

        private ManageCoursesDbContext GetContext()
        {
            Config["PG_DATABASE"] += "-smoke";
            var context = ContextLoader.GetDbContext(Config);
            return context;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (LocalWebHost != null)
            {
                LocalWebHost.Stop();
            }

            var context = GetContext();
            context.Database.EnsureDeleted();
        }
    }
}