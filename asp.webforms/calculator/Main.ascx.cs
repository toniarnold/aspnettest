using asp.calculator.Control;
using asp.calculator.View;
using asplib.Model;
using asplib.View;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using MainRow = asplib.Model.Main;
using StorageEnum = asplib.View.Storage;

namespace asp.calculator
{
    public partial class Main : CalculatorControl
    {
        public string StorageLinkUrl { get; set; }
        public string StorageLinkClientID { get; set; }

        public string Encrypted
        {
            get
            {
                return (this.GetStorage() == StorageEnum.Database && this.GetEncryptDatabaseStorage()) ?
                    "&#x1f512;" : String.Empty;
            }
        }

        public static IEnumerable<MainRow> AllMainRows()
        {
            return MainRow.AllMainRows<Calculator>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.LoadMain();

            // Required if used in an UpdatePanel, as a download is a real PostBack
            var sm = ScriptManager.GetCurrent(this.Page);
            sm.RegisterPostBackControl(this.shareButton);
        }

        /// <summary>
        /// Delete the Main row with the given session guid with a custom "Del" command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Del":
                    var session = Guid.Parse(e.CommandArgument.ToString());
                    using (var db = new ASP_DBEntities())
                    {
                        var sql = @"
                            DELETE FROM Main
                            WHERE session = @session
                        ";
                        var param = new SqlParameter("session", session);
                        db.Database.ExecuteSqlCommand(sql, param);
                    }
                    break;

                default:
                    throw new NotImplementedException(String.Format("e.CommandName='{0}'", e.CommandName));
            }
        }

        /// <summary>
        /// URL to open the page with an explicit session guid
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        protected string Url(Guid session)
        {
            var uri = this.Page.ResolveUrl("~/default.aspx");
            return String.Format("{0}?session={1}", uri, this.Server.UrlEncode(session.ToString()));
        }

        protected override void OnPreRender(EventArgs e)
        {
            // This State->View visibility mapping is the central axis of the SMC pattern.
            this.title.Visible = true;

            switch(this.State)
            {
                case var s when s == CalculatorContext.Map1.Calculate:
                    this.calculate.Visible = true;
                    break;
                case var s when s == CalculatorContext.Map1.Enter:
                    this.enter.Visible = true;
                    break;
                case var s when s == CalculatorContext.Map1.ErrorNumeric:
                    this.error.Visible = true;
                    this.error.Msg = "The input was not numeric.";
                    break;
                case var s when s == CalculatorContext.Map1.ErrorTuple:
                    this.error.Visible = true;
                    this.error.Msg = "Need two values on the stack to compute.";
                    break;
                case var s when s == CalculatorContext.Map1.ErrorEmpty:
                    this.error.Visible = true;
                    this.error.Msg = "Need a value on the stack to compute.";
                    break;
                case var s when s == CalculatorContext.Map1.Splash:
                    this.splash.Visible = true;
                    break;
                default:
                    throw new NotImplementedException(String.Format("this.State {0}", this.State));
            }

            this.footer.Visible = true;

            this.SaveMain();
            this.sessionDumpGridView.DataBind();    // reflect saved changes

            base.OnPreRender(e);
        }
    }
}