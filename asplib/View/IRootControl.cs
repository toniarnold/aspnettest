using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asplib.View
{
    /// <summary>
    /// Extension interface for a Control to access SetRoot() method
    /// </summary>
    public interface IRootControl
    {
    }

    public static class ControlRootExtension
    {
        public static void SetRoot(this IRootControl controlRoot)
        {
            ControlMainExtension.MainControl = (System.Web.UI.Control)controlRoot;
        }
    }
}
