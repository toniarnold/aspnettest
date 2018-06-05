using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using asplib.View;

namespace minimal
{
    public partial class withstorage : System.Web.UI.Page, IStorageControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.LoadStorage();
            this.contentTextBox.Focus();
        }

        protected void submitButton_Click(object sender, EventArgs e)
        {
            this.contentList.Items.Add(new ListItem(contentTextBox.Text));
            this.contentTextBox.Text = String.Empty;
        }

        protected void storageList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        protected override void OnPreRender(EventArgs e)
        {
            this.SaveStorage();
            base.OnPreRender(e);
        }
    }
}