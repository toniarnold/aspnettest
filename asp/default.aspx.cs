﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using asplib.View;
using iie;


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
            testRunner.Run("testie");

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