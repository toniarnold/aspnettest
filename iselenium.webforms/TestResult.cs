using System;
using System.Web.UI.WebControls;

namespace iselenium
{
    /// <summary>
    /// Test result summary encapsulating intermediate storage of the ResultString
    /// in the ViewState for displaying it when clicked.
    /// </summary>
    public class TestResult : LinkButton
    {
        public TestResult()
        {
            this.Load += Page_Load;
            this.Click += TestResult_Click;
        }

        public string ResultString
        {
            get { return (string)this.ViewState["RESULTSTRING"]; }
            set { this.ViewState["RESULTSTRING"] = value; }
        }

        /// <summary>
        /// Directly render the result as an application/xml response
        /// </summary>
        public void RenderTestResult()
        {
            this.Page.Response.Clear();
            this.Page.Response.AddHeader("Content-Type", "application/xml");
            this.Page.Response.Write(this.ResultString);
            this.Page.Response.End();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Attributes["style"] = "text-decoration: none;";
        }

        protected void TestResult_Click(object sender, EventArgs e)
        {
            this.RenderTestResult();
        }
    }
}