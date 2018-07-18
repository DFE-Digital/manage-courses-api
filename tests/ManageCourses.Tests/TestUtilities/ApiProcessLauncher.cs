using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.Tests.TestUtilities
{
    public class ApiProcessLauncher
    {
        public DisposableWebHost LaunchApiLocally(IConfiguration config)
        {            
            var webHost = WebHost.CreateDefaultBuilder()
                .UseStartup<ManageCourses.Api.Startup>()
                .UseConfiguration(config)
                .Build(); 
                

            webHost.RunAsync();
            Thread.Sleep(5000); //:( 

            return new DisposableWebHost(webHost);
        }

        public class DisposableWebHost : IDisposable
        {
            private IWebHost theHost = null;

            public DisposableWebHost(IWebHost webHost)
            {
                theHost = webHost;
            }

            public string Address => (theHost.ServerFeatures.First(x => x.Value is IServerAddressesFeature).Value as IServerAddressesFeature).Addresses.FirstOrDefault();
            
            public void Dispose()
            {
                if (theHost != null)
                {
                    theHost.StopAsync().Await();
                    theHost = null;
                }
            }
        }

    }
}