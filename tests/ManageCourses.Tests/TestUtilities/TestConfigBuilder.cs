using System;
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
            .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"Config/appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .AddUserSecrets<ApiEndpointTests>()
            .AddEnvironmentVariables()
            .Build();

            return config;
        }
    }
}
