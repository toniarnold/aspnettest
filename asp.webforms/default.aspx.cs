using asplib.View;
using iselenium;
using System;
using System.Web.UI;

namespace asp
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack && !String.IsNullOrEmpty(this.Request.QueryString["storage"]))
            {
                this.ViewState["storage_override"] = this.Request.QueryString["storage"];
            }
            var viewstateStorage = (string)this.ViewState["storage_override"];
            if (!String.IsNullOrEmpty(viewstateStorage))
            {
                // by itself non-persistent:
                this.calculator.SetStorage(this.Request.QueryString["storage"]);
            }
        }

        protected void testButton_Click(object sender, ImageClickEventArgs e)
        {
            var testRunner = new TestRunner(this.Request.Url.Port);
            testRunner.Run("asptest.webforms");
            if (TestRunner.Passed)
            {
                this.testResult.Text = testRunner.SummaryHtml;
            }
            else
            {
                this.testResult.RenderTestResult(TestRunner.ResultXml);
            }
        }
    }
}