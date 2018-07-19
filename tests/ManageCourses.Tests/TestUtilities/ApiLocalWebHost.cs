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
    public class ApiLocalWebHost
    {        
        private readonly IWebHost theHost;
        private int hasLaunched = 0;

        public ApiLocalWebHost(IConfiguration config)
        {
            theHost = WebHost.CreateDefaultBuilder()
                .UseStartup<ManageCourses.Api.Startup>()
                .UseConfiguration(config)
                .Build();                 
        }

        public ApiLocalWebHost Launch()
        {
            bool shouldLaunch = 0 == Interlocked.Exchange(ref hasLaunched, 1);

            if (shouldLaunch)
            {
                theHost.RunAsync();

                // HACK!
                // The Web Host needs a bit of time to warm up befor it serves requests
                // Rather than polling for readiness, we'll just wait for a few seconds
                Thread.Sleep(5000);
            }

            return this;
        }

        public void Stop()
        {
            bool shouldStop = 1 == Interlocked.Exchange(ref hasLaunched, 0);
            if (shouldStop)
            {                
                theHost.StopAsync().Result;
            }
        }

        public string Address 
        {
            get
            {
                if (hasLaunched == 0) 
                {
                    throw new InvalidOperationException("Called Address on unlaunched web host");
                }

                var addressesFeature = theHost.ServerFeatures.Get<IServerAddressesFeature>();

                return addressesFeature.Addresses.First();
            }
        }

    }
}