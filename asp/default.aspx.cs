using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using iie;


namespace asp
{
    public partial class _default : System.Web.UI.Page
    {
        protected void testButton_Click(object sender, ImageClickEventArgs e)
        {
            var testRunner = new TestRunner(this.Request.Url.Port);
            testRunner.Run("testie");

            this.Response.Clear();
            this.Response.AddHeader("Content-Type", "application/xml");
            this.Response.Write(testRunner.ResultString);
            this.Response.End();
        }
    }
}