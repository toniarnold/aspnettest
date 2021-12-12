using asplib.Remoting;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebSharper;

namespace iselenium
{
    /// <summary>
    /// WebSharper remoting server invoking the TestRunner
    /// </summary>
    public static class RemoteTestRunner
    {
        /// <summary>
        /// JS Value class
        /// </summary>
        public class TestResult
        {
            public bool Passed;
            public List<string> Summary;
        }

        /// <summary>
        /// Runner to be called asynchronously from a WebSharper remoting method,
        /// as it requires WebSharper.Web.Remoting.GetContext()
        /// </summary>
        /// <param name="testproject"></param>
        /// <returns></returns>
        [Remote]
        public static async Task<TestResult> Run(string testproject)
        {
            var testRunner = new TestRunner(
                RemotingContext.Configuration,
                RemotingContext.Environment,
                RemotingContext.Port);
            await Task.Run(() => testRunner.Run(testproject));
            var result = new TestResult();
            result.Passed = TestRunner.Passed;
            result.Summary = testRunner.Summary;
            return result;
        }

        /// <summary>
        /// Runner to be called synchronously from a WebSharper sitelet endpoint
        /// </summary>
        /// <param name="testproject"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static TestResult Run(string testproject, int port)
        {
            var testRunner = new TestRunner(
                RemotingContext.Configuration,
                RemotingContext.Environment,
                port);
            testRunner.Run(testproject);
            var result = new TestResult();
            result.Passed = TestRunner.Passed;
            result.Summary = testRunner.Summary;
            return result;
        }
    }
}