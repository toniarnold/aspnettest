using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace iselenium
{
    public interface ITestServer : ITestServerBase
    {
    }

    public static class TestServerExtension
    {
        /// <summary>
        /// Get the configuration with name "appsettings.json"
        /// </summary>
        /// <returns></returns>
        public static IConfiguration GetConfig(this ITestServer inst)
        {
            return GetConfig(inst, "appsettings.json");
        }

        /// <summary>
        /// Get the configuration with the given name
        /// </summary>
        /// <param name="jsonFile"></param>
        /// <returns></returns>
        public static IConfiguration GetConfig(this ITestServer inst, string jsonFile)
        {
            return new ConfigurationBuilder()
                .SetBasePath(TestContext.CurrentContext.WorkDirectory)
                .AddJsonFile(jsonFile, optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        /// <summary>
        /// Start the web application .exe as web server according to appsettings.json:
        /// Server: path to the server.exe (the .NET Core binary)
        /// Root: server application root directory
        /// Port: port to listen on
        /// RequestTimeout: expected duration of all tests in sec
        /// ServerStartTimeout: expected start time of the server in sec
        /// </summary>
        public static void StartServer(this ITestServer inst)
        {
            var config = GetConfig(inst);
            StartServer(inst, config);
        }

        /// <summary>
        /// Start the web application .exe as web server with optional parameters
        /// overriding the default configuration in appsettings.json:
        /// Server: path to the server.exe (the .NET Core binary)
        /// Root: server application root directory
        /// Port: port to listen on
        /// RequestTimeout: expected duration of all tests in sec
        /// ServerStartTimeout: expected start time of the server in sec
        /// Can be called multiple times to start auxiliary service processes.
        /// </summary>
        /// <param name="config">default configuration</param>
        /// <param name="server">explicit path to the server.exe (the .NET Core binary)</param>
        /// <param name="root">explicit server application root directory</param>
        /// <param name="port">explicit port to listen on</param>
        /// <param name="timeout">explicit expected duration of all tests in sec</param>
        /// <param name="servertimeout">expected start time of the server in sec</param>
        public static void StartServer(this ITestServer inst, IConfiguration config,
                                       string server = null, string root = null, int? port = null,
                                       int? timeout = null, int? servertimeout = null)
        {
            string cserver = server ?? config["Server"];
            string croot = root ?? config["Root"];
            int cport = port ?? config.GetValue<int>("Port");
            int ctimeout = timeout ?? config.GetValue<int>("RequestTimeout");
            int cservertimeout = servertimeout ?? config.GetValue<int>("ServerStartTimeout");

            var info = new ProcessStartInfo();
            info.FileName = Path.GetFullPath(Path.Join(TestContext.CurrentContext.WorkDirectory, cserver));
            info.Arguments = String.Format("--urls=http://localhost:{0}/", cport);
            info.WorkingDirectory = Path.GetFullPath(Path.Join(TestContext.CurrentContext.WorkDirectory, croot));
            info.UseShellExecute = true;
            if (inst.ServerProcesses == null)
                inst.ServerProcesses = new List<Process>();
            inst.ServerProcesses.Add(Process.Start(info));
            TestServerExtensionBase.WaitForServerPort(cport, cservertimeout);
            SeleniumExtensionBase.OutOfProcess = true;
            SeleniumExtensionBase.Port = cport;
            SeleniumExtensionBase.RequestTimeout = ctimeout;
            if (inst.driver != null) // SeleniumTest
            {
                inst.driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(ctimeout); // too late after OneTimeSetUBrowser()
                TestServerIPC.CreateOrOpenMmmfs();  // Create as parent process
            }
        }

        /// <summary>
        /// Kill the web server process and eventual auxiliary processes
        /// </summary>
        /// <param name="inst"></param>
        public static void StopServer(this ITestServer inst)
        {
            TestServerIPC.Dispose();
            if (inst.ServerProcesses != null)
            {
                foreach (var process in inst.ServerProcesses)
                {
                    try
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                    catch { }
                    finally
                    {
                        process.Dispose();
                    }
                }
                inst.ServerProcesses = null;
            }
        }
    }
}