using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        /// Start IIS Express application with optional parameters
        /// overriding the default configuration in App.config
        /// Server: IIS Express, optionally overrides %PROGRAMFILES%\IIS Express\iisexpress.exe
        /// Root: server application root directory
        /// Port: port to listen on
        /// RequestTimeout: expected duration of all tests in sec
        /// ServerStartTimeout: expected start time of the server in sec
        /// Can be called multiple times to start auxiliary service processes.
        /// </summary>
        /// <param name="server">IIS Express, usually %PROGRAMFILES%\IIS Express\iisexpress.exe</param>
        /// <param name="root">server application root directory</param>
        /// <param name="port">port to listen on</param>
        /// <param name="timeout">expected duration of all tests in sec</param>
        /// <param name="servertimeout">expected start time of the server in sec</param>
        public static void StartServer(this ITestServer inst, string server = null, string root = null,
                                       int? port = null, int? timeout = null, int? servertimeout = null)
        {
            string cserver = server ?? ConfigurationManager.AppSettings["Server"] ??
                                            @"%PROGRAMFILES%\IIS Express\iisexpress.exe";
            string croot = root ?? ConfigurationManager.AppSettings["Root"];
            int cport = port ?? int.Parse(ConfigurationManager.AppSettings["Port"]);
            int ctimeout = timeout ?? (String.IsNullOrWhiteSpace(
                    ConfigurationManager.AppSettings["RequestTimeout"]) ? 1 :
                    int.Parse(ConfigurationManager.AppSettings["RequestTimeout"]));
            int cservertimeout = servertimeout ?? int.Parse(ConfigurationManager.AppSettings["ServerStartTimeout"]);

            var info = new ProcessStartInfo();
            info.FileName = cserver.Replace("%PROGRAMFILES%",
                                            System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
            var path = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.WorkDirectory, croot));
            info.Arguments = String.Format("/path:{0} /port:{1} /trace:error", path, cport);
            info.UseShellExecute = true;
            if (inst.ServerProcesses == null)
                inst.ServerProcesses = new List<Process>();
            inst.ServerProcesses.Add(Process.Start(info));
            TestServerExtensionBase.WaitForServerPort(cport, cservertimeout);
            SeleniumExtensionBase.OutOfProcess = true;
            SeleniumExtensionBase.Port = cport;
            SeleniumExtensionBase.RequestTimeout = ctimeout;
            inst.driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(ctimeout);   // too late after OneTimeSetUBrowser()

            TestServerIPC.CreateOrOpenMmmfs();  // Create as parent process
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