using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using asplib.Controllers;
using minimal.Models;
using asplib.Model;

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
