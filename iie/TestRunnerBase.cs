using NUnit.Engine;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace iie
{
    public abstract class TestRunnerBase : ITestEventListener
    {
        public TestRunnerBase(int port)
        {
            IEExtensionBase.Port = port;
            // Initialize static fields when creating a new instance
            Reports = new List<string>();
            Result = null;
        }

        protected static List<string> Reports { get; set; }
        protected static XmlNode Result { get; set; }

        /// <summary>
        /// Return the result as XML string, static for later retrieval after
        /// the tests ran
        /// </summary>
        public static string ResultString
        {
            get
            {
                using (var stringwriter = new StringWriter())
                using (var xmlwriter = new XmlTextWriter(stringwriter))
                {
                    Result.WriteTo(xmlwriter);
                    return stringwriter.ToString();
                }
            }
        }

        /// <summary>
        /// True when the test suite passed
        /// </summary>
        public bool Passed
        {
            get
            {
                return Result.Attributes["result"].Value == "Passed";
            }
        }

        /// <summary>
        /// Summary to display instead of the result xml file.
        /// </summary>
        public string PassedString
        {
            get
            {
                return string.Format("Passed<br/>Tests: {0}<br/>Asserts: {1}<br/>Duration: {2}",
                    Result.Attributes["total"].Value,
                    Result.Attributes["asserts"].Value,
                    Result.Attributes["duration"].Value
                    );
            }
        }

        public void OnTestEvent(string report)
        {
            Reports.Add(report);
        }

        /// <summary>
        /// Directly configure the test engine without dependency on a specific
        /// configuration framework.
        /// </summary>
        protected void Configure(int requestTimeout)
        {
            IEExtensionBase.RequestTimeoutMS = requestTimeout * 1000;
        }
    }
}