using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace iie
{
    public abstract class TestRunnerBase
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
        /// Return the result as XML string
        /// </summary>
        public string ResultString
        {
            get { return StaticResultString; }
        }

        /// <summary>
        /// Return the result as XML string, static for later retrieval after
        /// the tests ran (ASP.NET Core without internal ViewState)
        /// </summary>
        public static string StaticResultString
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
        /// Summary to display instead of the result XML file as list of lines
        /// </summary>
        public List<string> Summary
        {
            get
            {
                var retwal = new List<string>();
                retwal.Add("Passed");
                retwal.Add(string.Format("Tests: {0}", Result.Attributes["total"].Value));
                retwal.Add(string.Format("Asserts: {0}", Result.Attributes["asserts"].Value));
                retwal.Add(string.Format("Duration: {0}", Result.Attributes["duration"].Value));
                return retwal;
            }
        }

        /// <summary>
        /// Summary to display instead of the result XML file as HTML snippet
        /// </summary>
        public string SummaryHtml
        {
            get
            {
                return string.Join("<br />", this.Summary);
            }
        }

        [Obsolete("Replaced with SummaryHtml", true)]
        public string PassedString
        {
            get { return this.SummaryHtml; }
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