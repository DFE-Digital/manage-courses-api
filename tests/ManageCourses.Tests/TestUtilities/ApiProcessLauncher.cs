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
            var dirSep = System.IO.Path.DirectorySeparatorChar;
            
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run -p ..{dirSep}..{dirSep}..{dirSep}..{dirSep}..{dirSep}src{dirSep}ManageCourses.Api",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true  
            };

            foreach(var foo in config.AsEnumerable())
            {
                startInfo.EnvironmentVariables[foo.Key.Replace(":","__")] = foo.Value;
            }
            
            var process = new Process()
            {
                StartInfo = startInfo
            };

            process.OutputDataReceived += (sender, args) => Console.WriteLine("api process: {0}", args.Data);
            process.ErrorDataReceived += (sender, args) => Console.WriteLine("api process error: {0}", args.Data);

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