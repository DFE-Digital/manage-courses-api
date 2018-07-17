using System;
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
            // Logging setup based on https://github.com/serilog/serilog-aspnetcore
            // and https://github.com/serilog/serilog-settings-configuration

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console() // todo: This should be respecting the value in appsettings, not sure why that's not working
                .CreateLogger();

            try
            {
                Log.Information("Starting web host");
                BuildWebHost(args).Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog()
                .Build();
    }
}
