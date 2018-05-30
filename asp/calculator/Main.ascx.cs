/*
 * R web control containing the state machine
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using MainRow = asplib.Model.Main;
using asplib.View;
using StorageEnum = asplib.View.Storage;

using asp.calculator.Control;
using asp.calculator.View;

namespace asp.calculator
{
    public partial class Main : CalculatorControl
    {
        public string StorageLinkUrl
        {
            set { this.storageLink.NavigateUrl = value; }
        }

        public static IEnumerable<MainRow> AllMainRows()
        {
            return MainRow.AllMainRows<Calculator>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.LoadMain();
        }

        /// <summary>
        /// Redirect to the default page with the session-Main Calculator overriden with ?session=guid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch(e.CommandName)
            {
                case "Open":
                    var session = e.CommandArgument.ToString();
                    var uri = this.Page.ResolveUrl("~/default.aspx");
                    var url = string.Format("{0}?session={1}", uri, this.Server.UrlEncode(session));
                    this.Response.Redirect(url, true);
                    break;
                default:
                    throw new NotImplementedException(String.Format("e.CommandName='{0}'", e.CommandName));
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            var isDatabaseStorage = (this.GetStorage() == StorageEnum.Database);
            this.headlinkDiv.Visible = isDatabaseStorage;
            this.sessionDumpGridView.Visible = isDatabaseStorage;

            this.title.Visible = true;

            if (this.State == CalculatorContext.Map1.Calculate)
            {
                this.calculate.Visible = true;
            }
            else if (this.State == CalculatorContext.Map1.Enter)
            {
                this.enter.Visible = true;
            }
            else if (this.State == CalculatorContext.Map1.ErrorNumeric)
            {
                this.error.Visible = true;
                this.error.Msg = "The input was not numeric.";
            }
            else if (this.State == CalculatorContext.Map1.ErrorTuple)
            {
                this.error.Visible = true;
                this.error.Msg = "Need two values on the stack to compute.";
            }
            else if (this.State == CalculatorContext.Map1.ErrorEmpty)
            {
                this.error.Visible = true;
                this.error.Msg = "Need a value on the stack to compute.";
            }
            else if (this.State == CalculatorContext.Map1.Splash)
            {
                this.splash.Visible = true;
            }
            else
            {
                throw new NotImplementedException(String.Format("this.State {0}", this.State));
            }

            this.footer.Visible = true;

            this.SaveMain();
            this.sessionDumpGridView.DataBind();    // reflect saved changes

            base.OnPreRender(e);
        }
    }
}