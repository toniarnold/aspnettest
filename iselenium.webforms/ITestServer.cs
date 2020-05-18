using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Configuration;
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
        /// Start IIS Express according to App.config
        /// Server: IIS Express, optionally overrides %PROGRAMFILES%\IIS Express\iisexpress.exe
        /// Root: server application root directory
        /// Port: port to listen on
        /// Timeout: expected duration of all tests
        /// </summary>
        /// <param name="inst"></param>
        public static void StartServer(this ITestServer inst)
        {
            var server = ConfigurationManager.AppSettings["Server"] ??
                            @"%PROGRAMFILES%\IIS Express\iisexpress.exe";
            StartServer(inst, server, ConfigurationManager.AppSettings["Root"],
                        int.Parse(ConfigurationManager.AppSettings["Port"]),
                        int.Parse(ConfigurationManager.AppSettings["Timeout"]));
        }

        /// <summary>
        /// Start IIS Express with these parameters
        /// </summary>
        /// <param name="server">IIS Express, usually %PROGRAMFILES%\IIS Express\iisexpress.exe</param>
        /// <param name="root">server application root directory</param>
        /// <param name="port">port to listen on</param>
        /// <param name="timeout">expected duration of all tests</param>
        public static void StartServer(this ITestServer inst, string server, string root, int port, int timeout)
        {
            var info = new ProcessStartInfo();
            info.FileName = server.Replace("%PROGRAMFILES%",
                                            System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
            var path = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.WorkDirectory, root));
            info.Arguments = String.Format("/path:{0} /port:{1} /trace:error", path, port);
            info.UseShellExecute = true;
            inst.ServerProcess = Process.Start(info);
            SeleniumExtensionBase.OutOfProcess = true;
            SeleniumExtensionBase.Port = port;
            SeleniumExtensionBase.RequestTimeout = timeout;
            inst.driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(timeout);
        }

        public static void StopServer(this ITestServer inst)
        {
            inst.ServerProcess.Kill();
        }
    }
}