using asp.Models;
using Microsoft.AspNetCore.Mvc;

namespace asp.Views.Calculator.Components.Calculate
{
    public class Calculate : ViewComponent
    {
        public IViewComponentResult Invoke(CalculatorViewModel model)
        {
            return View(model);
        }
    }
}