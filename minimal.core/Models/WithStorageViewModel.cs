using asplib.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;

namespace minimal.Models
{
    [Serializable]
    public class WithStorageViewModel
    {
        public Storage Storage { get; set; }
        public string ContentTextBox { get; set; }

        [BindNever]
        public List<string> Content { get; set; }

        public WithStorageViewModel()
        {
            this.Content = new List<string>();
        }
    }
}