using asplib.Model;
using System;
using System.Collections.Generic;

namespace minimal
{
    /// <summary>
    /// Pure storage class for the minimal dependency setup
    /// </summary>
    [Serializable]
    [Clsid("440ACD97-DAED-4AF7-BD2D-7DBDCBCF41D5")]
    public class ContentStorage
    {
        public List<string> Content = new List<string>();
    }
}