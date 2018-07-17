using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.Tests.TestUtilities
{
    public class ApiProcessLauncher
    {
        public DisposableProcess LaunchApiLocally(IConfiguration config)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run -p ..\\..\\..\\..\\..\\src\\ManageCourses.Api",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true  
            };

            foreach(var foo in config.AsEnumerable())
            {
                startInfo.EnvironmentVariables[foo.Key] = foo.Value;
            }
            
            var process = new Process()
            {
                StartInfo = startInfo
            };
            
            process.Start();
            Thread.Sleep(10000); // :(
            return new DisposableProcess(process);
        }

        public class DisposableProcess : IDisposable
        {
            private Process theProcess = null;

            public DisposableProcess(Process process)
            {
                theProcess = process;
            }

            public void Dispose()
            {
                if (theProcess != null && !theProcess.HasExited) {
                    theProcess.Kill();
                }    
            }
        }

    }
}