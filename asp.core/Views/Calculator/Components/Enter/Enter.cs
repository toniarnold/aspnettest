using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using asp.Models;

namespace asp.Views.Calculator.Components.Enter
{
    public class Enter : ViewComponent
    {
        public IViewComponentResult Invoke(CalculatorViewModel model)
        {
            return View(model);
        }
    }
}
