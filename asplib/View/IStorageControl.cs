using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asplib.View
{
    /// <summary>
    /// Extension interface for a Control to access to the LoadStorage()/SaveStorage() methods
    /// </summary>
    public interface IStorageControl
    {
    }

    public static class ControlStorageExtension
    {
        public static void LoadStorage(this IStorageControl controlStorage)
        {
            ControlMainExtension.MainControl = (System.Web.UI.Control)controlStorage;
        }

        public static void SaveStorage(this IStorageControl controlStorage)
        {

        }
    }
}
