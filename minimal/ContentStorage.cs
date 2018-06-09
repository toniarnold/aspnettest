using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace minimal
{
    /// <summary>
    /// Pure storage class for the minimal dependency setup
    /// </summary>
    [Serializable]
    public class ContentStorage
    {
        public List<string> Content = new List<string>();
    }
}