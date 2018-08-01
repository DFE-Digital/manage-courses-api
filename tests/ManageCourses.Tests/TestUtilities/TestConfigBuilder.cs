using System.IO;
using GovUk.Education.ManageCourses.Tests.SmokeTests;
using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.Tests.TestUtilities
{
    internal class TestConfigBuilder
    {
        public static IConfigurationRoot BuildTestConfig()
        {
            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddUserSecrets<ApiEndpointTests>()
            .AddEnvironmentVariables()
            .Build();

            return config;
        }
    }
}
