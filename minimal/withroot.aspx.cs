using asplib.View;
using System;
using System.Web.UI.WebControls;

namespace minimal
{
    public partial class withroot : System.Web.UI.Page, IRootControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.SetRoot();
            this.contentTextBox.Focus();
        }

        protected void submitButton_Click(object sender, EventArgs e)
        {
            this.contentList.Items.Add(new ListItem(contentTextBox.Text));
            this.contentTextBox.Text = String.Empty;
        }
    }
}