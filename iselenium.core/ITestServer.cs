using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Diagnostics;
using System.IO;

namespace iselenium
{
    public interface ITestServer
    {
        Process ServerProcess { get; set; }
        IWebDriver driver { get; set; }
    }

    public static class TestServerExtension
    {
        /// <summary>
        /// Start the web server according to appsettings.json:
        /// Server: path to the server.exe (the .NET Core binary)
        /// Root: server application root directory
        /// Port: port to listen on
        /// RequestTimeout: expected duration of all tests
        /// </summary>
        public static void StartServer(this ITestServer inst)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(TestContext.CurrentContext.WorkDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            StartServer(inst, config["Server"], config["Root"],
                        config.GetValue<int>("Port"), config.GetValue<int>("RequestTimeout"));
        }

        /// <summary>
        /// Start the web server with these parameters
        /// </summary>
        /// <param name="server">path to the server.exe (the .NET Core binary)</param>
        /// <param name="root">server application root directory</param>
        /// <param name="port">port to listen on</param>
        /// <param name="timeout">expected duration of all tests</param>
        public static void StartServer(this ITestServer inst, string server, string root, int port, int timeout)
        {
            var info = new ProcessStartInfo();
            info.FileName = Path.GetFullPath(Path.Join(TestContext.CurrentContext.WorkDirectory, server));
            info.WorkingDirectory = Path.GetFullPath(Path.Join(TestContext.CurrentContext.WorkDirectory, root));
            info.UseShellExecute = true;
            inst.ServerProcess = Process.Start(info);
            SeleniumExtensionBase.OutOfProcess = true;
            SeleniumExtensionBase.Port = port;
            SeleniumExtensionBase.RequestTimeout = timeout;
            inst.driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(timeout); // too late after OneTimeSetUBrowser()
        }

        public static void StopServer(this ITestServer inst)
        {
            inst.ServerProcess.Kill();
        }
    }
}