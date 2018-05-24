using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using asplib.View;


namespace asp
{
    /// <summary>
    /// Reliably clear (i.e. without race conditions using HttpContext.Current)
    /// Storage.Session or Storag.Database between GUI tests
    /// </summary>
    public partial class clear : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Storage storage;
            Enum.TryParse(this.Request.QueryString["storage"], out storage);
            switch(storage)
            {
                case Storage.Session:
                    this.Session.Clear();
                    break;
                default:
                    throw new NotImplementedException(String.Format("Storage {0}", storage));
            }
        }
    }
}