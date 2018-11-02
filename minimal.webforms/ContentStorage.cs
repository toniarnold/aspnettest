using System;
using System.Collections.Generic;

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