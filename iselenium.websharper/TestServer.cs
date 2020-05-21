using asplib.Remoting;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebSharper;

namespace iselenium
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
#pragma warning disable CS0246 // Der Typ- oder Namespacename "TestRunner" wurde nicht gefunden (möglicherweise fehlt eine using-Direktive oder ein Assemblyverweis).
            var testRunner = new TestRunner(
#pragma warning restore CS0246 // Der Typ- oder Namespacename "TestRunner" wurde nicht gefunden (möglicherweise fehlt eine using-Direktive oder ein Assemblyverweis).
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