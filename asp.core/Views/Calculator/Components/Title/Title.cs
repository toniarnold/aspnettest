using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Controller = asp.Controllers.Calculator;
using asp.Models;

namespace asp.Views.Calculator.Components.Title
{
    public class Title : ViewComponent
    {
        public IViewComponentResult Invoke(CalculatorViewModel model)
        {
            return View(model);
        }
    }
}
