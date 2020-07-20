using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;

namespace iselenium
{
    public interface ITestServerBase
    {
        List<Process> ServerProcesses { get; set; }
        IWebDriver driver { get; set; }
    }

    public static class TestServerExtensionBase
    {
        /// <summary>
        /// Poll for a successful TCP connection at the given port
        /// </summary>
        /// <param name="port">port to listen on</param>
        /// <param name="timeout">expected duration of all tests in sec</param>
        public static void WaitForServerPort(int port, int servertimeout)
        {
            int interval = 1000;    // 1 sec
            int times = servertimeout;
            bool success = false;
            using (var client = new TcpClient())
            {
                while (!success && times >= 0)
                {
                    try
                    {
                        var asyncResult = client.BeginConnect("localhost", port, null, null);
                        while (!asyncResult.AsyncWaitHandle.WaitOne(interval)) { }
                        client.EndConnect(asyncResult);
                    }
                    catch
                    {
                        servertimeout--;
                    }
                    success = true;
                }
            }
            if (!success)
            {
                throw new Exception(String.Format("Server on Port {0} not reachable within {1} seconds",
                                                    port, servertimeout));
            }
        }
    }
}