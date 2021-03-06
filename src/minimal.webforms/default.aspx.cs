﻿using iselenium;
using System.Web.UI;

namespace minimal
{
    public partial class _default : System.Web.UI.Page
    {
        protected void testButton_Click(object sender, ImageClickEventArgs e)
        {
            var testRunner = new TestRunner(this.Request.Url.Port);
            testRunner.Run("minimaltest.webforms");
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