using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using asplib.View;

namespace minimal
{
    public class MaliciousContentException : Exception
    {
        public MaliciousContentException(string msg) : base(msg) { }
    }

    public partial class withstorage : System.Web.UI.Page, IStorageControl<ContentStorage>
    {
        public ContentStorage Main { get; set; }

        public new StateBag ViewState
        {
            get { return base.ViewState; }
        }

        /// <summary>
        /// For dynamic assignment in the .ascx
        /// </summary>
        public string Storage
        {
            get { return this.GetStorage().ToString(); }
            set { this.SetStorage(value); }
        }

        /// <summary>
        /// The local storage type field as enum
        /// </summary>
        public Storage? SessionStorage { get; set; }


        protected void Page_Load(object sender, EventArgs e)
        {
            this.SetStorage(storageList.SelectedValue);
            this.LoadMain();
            this.contentTextBox.Focus();
        }

        protected void submitButton_Click(object sender, EventArgs e)
        {
            if (String.Compare(this.contentTextBox.Text, "except", true) == 0)
            {
                throw new MaliciousContentException("Malicious Content Exception");
            }
            this.Main.Content.Add(contentTextBox.Text);
            this.contentTextBox.Text = String.Empty;
        }

        protected void storageList_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SetStorage(storageList.SelectedValue);
        }

        protected override void OnPreRender(EventArgs e)
        {
            // EnableViewState="false" and persist the storage ourselves
            this.contentList.DataSource = this.Main.Content;
            this.contentList.DataBind();

            this.SaveMain();
            base.OnPreRender(e);
        }
    }
}