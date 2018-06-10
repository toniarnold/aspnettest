using iie;
using System.Web.UI;

namespace minimal
{
    public partial class _default : System.Web.UI.Page
    {
        protected void testButton_Click(object sender, ImageClickEventArgs e)
        {
            var testRunner = new TestRunner(this.Request.Url.Port);
            testRunner.Run("minimaltest");

            if (testRunner.Passed)
            {
                this.testResultLabel.Text = testRunner.PassedString;
            }
            else
            {
                this.Response.Clear();
                this.Response.AddHeader("Content-Type", "application/xml");
                this.Response.Write(testRunner.ResultString);
                this.Response.End();
            }
        }
    }
}