using System;
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
        private readonly IWebHost _theHost;
        private int _hasLaunched;

        public ApiLocalWebHost(IConfiguration config)
        {
            _theHost = WebHost.CreateDefaultBuilder()
                .UseStartup<Api.Startup>()
                .UseConfiguration(config)
                .Build();                 
        }

        public ApiLocalWebHost Launch()
        {
            bool shouldLaunch = 0 == Interlocked.Exchange(ref _hasLaunched, 1);

            if (shouldLaunch)
            {
                _theHost.RunAsync();

                // HACK!
                // The Web Host needs a bit of time to warm up befor it serves requests
                // Rather than polling for readiness, we'll just wait for a few seconds
                Thread.Sleep(5000);
            }

            return this;
        }

        public void Stop()
        {
            bool shouldStop = 1 == Interlocked.Exchange(ref _hasLaunched, 0);
            if (shouldStop)
            {                
                _theHost.StopAsync().Wait();
            }
        }

        public string Address 
        {
            get
            {
                if (_hasLaunched == 0) 
                {
                    throw new InvalidOperationException("Called Address on unlaunched web host");
                }

                var addressesFeature = _theHost.ServerFeatures.Get<IServerAddressesFeature>();

                return addressesFeature.Addresses.First();
            }
        }

    }
}