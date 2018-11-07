using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using asp.Models;

namespace asp.Views.Calculator.Components.Splash
{
    public class Splash : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
