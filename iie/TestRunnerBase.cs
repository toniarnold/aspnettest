using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Engine;
using NUnit.Framework;
using System.Xml;
using System.IO;

namespace iie
{
    public abstract class TestRunnerBase : ITestEventListener
    {
        public TestRunnerBase(int port)
        {
            IEExtensionBase.Port = port;
        }

        protected XmlNode result;
        protected List<string> reports = new List<string>();

        /// <summary>
        /// Return the result as XML string
        /// </summary>
        public string ResultString
        {
            get
            {
                using (var stringwriter = new StringWriter())
                using (var xmlwriter = new XmlTextWriter(stringwriter))
                {
                    this.result.WriteTo(xmlwriter);
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
                return this.result.Attributes["result"].Value == "Passed";
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
                    this.result.Attributes["total"].Value,
                    this.result.Attributes["asserts"].Value,
                    this.result.Attributes["duration"].Value
                    );
            }
        }

        public List<string> Reports
        {
            get { return this.reports; }
        }

        public void OnTestEvent(string report)
        {
            this.reports.Add(report);
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
