using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace minimal.Models
{
    public class WithStaticViewModel
    {
        public WithStaticViewModel()
        {
            this.Content = new List<string>();
        }

        public string ContentTextBox { get; set; }
        public List<string> Content { get; set; }

        [HiddenInput]   // not used, see IActionResult Submit(WithStaticViewModel model)
        public string ContentString
        {
            get
            {
                return String.Join("\n", this.Content);
            }
            set
            {
                if (value != null)  // initialization of the form
                {
                    this.Content = value.Split("\n", StringSplitOptions.RemoveEmptyEntries).ToList();
                }
            }
        }
    }
}