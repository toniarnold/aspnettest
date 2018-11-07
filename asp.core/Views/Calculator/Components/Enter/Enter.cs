using asp.Models;
using Microsoft.AspNetCore.Mvc;

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