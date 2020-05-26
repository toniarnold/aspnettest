using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
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
        /// Start the web application .exe as web server according to appsettings.json
        /// Server: path to the server.exe (the .NET Core binary)
        /// Root: server application root directory
        /// Port: port to listen on
        /// RequestTimeout: expected duration of all tests in sec
        /// ServerStartTimeout: expected start time of the server in sec
        /// </summary>
        public static void StartServer(this ITestServer inst)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(TestContext.CurrentContext.WorkDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            StartServer(inst, config["Server"], config["Root"], config.GetValue<int>("Port"),
                        config.GetValue<int>("RequestTimeout"), config.GetValue<int>("ServerStartTimeout"));
        }

        /// <summary>
        /// Start the web application .exe as web server with these parameters
        /// </summary>
        /// <param name="server">path to the server.exe (the .NET Core binary)</param>
        /// <param name="root">server application root directory</param>
        /// <param name="port">port to listen on</param>
        /// <param name="timeout">expected duration of all tests in sec</param>
        /// <param name="servertimeout">expected start time of the server in sec</param>
        public static void StartServer(this ITestServer inst, string server, string root,
                                        int port, int timeout, int servertimeout)
        {
            var info = new ProcessStartInfo();
            info.FileName = Path.GetFullPath(Path.Join(TestContext.CurrentContext.WorkDirectory, server));
            info.Arguments = String.Format("--urls=http://localhost:{0}/", port);
            info.WorkingDirectory = Path.GetFullPath(Path.Join(TestContext.CurrentContext.WorkDirectory, root));
            info.UseShellExecute = true;
            inst.ServerProcess = Process.Start(info);
            TestServerExtensionBase.WaitForServerPort(port, servertimeout);
            SeleniumExtensionBase.OutOfProcess = true;
            SeleniumExtensionBase.Port = port;
            SeleniumExtensionBase.RequestTimeout = timeout;
            inst.driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(timeout); // too late after OneTimeSetUBrowser()

            TestServerIPC.CreateOrOpenMmmfs();
        }
    }
}