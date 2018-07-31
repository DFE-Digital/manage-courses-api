using System.IO;
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
            var context = ContextLoader.GetDbContext(ContextLoader.IntegrationTestJson);

            context.Database.EnsureDeleted();
            context.Database.Migrate();

            config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(ContextLoader.IntegrationTestJson)
                .AddUserSecrets<ApiEndpointTests>()
                .AddEnvironmentVariables()
                .Build();

            localWebHost = new ApiLocalWebHost(config).Launch();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (localWebHost != null)
            {
                localWebHost.Stop();
            }
        }
    }
}