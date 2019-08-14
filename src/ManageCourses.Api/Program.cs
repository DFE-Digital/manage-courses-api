using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace GovUk.Education.ManageCourses.Api
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var configuration = GetConfiguration();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo
                .ApplicationInsightsTraces(configuration["APPINSIGHTS_INSTRUMENTATIONKEY"])
                .CreateLogger();

            var programLogger = Log.ForContext<Program>();

            try
            {
                programLogger.Information("Starting web host");
                BuildWebHost(args).Run();
                return 0;
            }
            catch (Exception ex)
            {
                programLogger.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .CaptureStartupErrors(true)
                .UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
                .UseKestrel(options =>
                {
                    options.AddServerHeader = false;
                })
                .UseApplicationInsights()
                .UseStartup<Startup>()
                .UseSentry(o =>
                {
                    o.MaxBreadcrumbs = 200;
                    o.MaxQueueItems = 100;
                    o.ShutdownTimeout = TimeSpan.FromSeconds(5);
                })
                .UseConfiguration(GetConfiguration())
                .Build();

        private static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"Config/appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
