using System;
using System.Web.UI.WebControls;

namespace iselenium
{
    /// <summary>
    /// Test result summary LinkButton
    /// </summary>
    public class TestResult : LinkButton
    {
        public TestResult()
        {
            this.Load += Page_Load;
            this.Click += TestResult_Click;
        }

        /// <summary>
        /// Directly render the result as an application/xml response
        /// </summary>
        public void RenderTestResult(string resultXml)
        {
            this.Page.Response.Clear();
            this.Page.Response.AddHeader("Content-Type", "application/xml");
            this.Page.Response.Write(resultXml);
            this.Page.Response.End();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Attributes["style"] = "text-decoration: none;";
        }

        protected void TestResult_Click(object sender, EventArgs e)
        {
            this.RenderTestResult(TestRunner.ResultXml);   // always the whole result
        }
    }
}