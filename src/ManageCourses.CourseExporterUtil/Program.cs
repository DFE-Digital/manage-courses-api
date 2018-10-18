using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.CourseExporterUtil
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var configuration = GetConfig();
            var publisher = new Publisher(configuration);
            publisher.Publish();
        }

        private static IConfiguration GetConfig()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddUserSecrets<Api.Startup>()
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
