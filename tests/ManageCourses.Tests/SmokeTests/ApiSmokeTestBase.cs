using GovUk.Education.ManageCourses.Tests.DbIntegration;
using GovUk.Education.ManageCourses.Tests.TestUtilities;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.SmokeTests
{
    public abstract class ApiSmokeTestBase : DbIntegrationTestBase
    {
        protected ApiLocalWebHost LocalWebHost;

        public override void OneTimeSetup()
        {
            LocalWebHost = new ApiLocalWebHost(Config).Launch();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            LocalWebHost?.Stop();
        }
    }
}
