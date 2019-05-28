using asplib;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebSharper;

namespace iie
{
    public static class TestServer
    {
        /// <summary>
        /// JS Value class
        /// </summary>
        public class TestResult
        {
            public bool Passed;
            public List<string> Summary;
        }

        [Remote]
        public static async Task<TestResult> Test(string testproject)
        {
            var testRunner = new TestRunner(
                RemotingContext.Configuration,
                RemotingContext.Environment,
                RemotingContext.Port);
            await Task.Run(() => testRunner.Run(testproject));
            var result = new TestResult();
            result.Passed = testRunner.Passed;
            result.Summary = testRunner.Summary;
            return result;
        }
    }
}