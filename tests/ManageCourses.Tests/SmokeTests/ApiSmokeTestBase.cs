using System.IO;
using GovUk.Education.ManageCourses.Tests.TestUtilities;
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
            config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("integration-tests.json")
                .AddUserSecrets<DataServiceExportTests>()
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