using iie;
using System.Web.UI;

namespace minimal
{
    public partial class _default : System.Web.UI.Page
    {
        protected void testButton_Click(object sender, ImageClickEventArgs e)
        {
            var testRunner = new TestRunner(this.Request.Url.Port);
            testRunner.Run("minimaltest.webforms");
            this.testResult.ResultString = testRunner.ResultString;

            if (testRunner.Passed)
            {
                this.testResult.Text = testRunner.PassedString;
            }
            else
            {
                this.testResult.RenderTestResult();
            }
        }
    }
}