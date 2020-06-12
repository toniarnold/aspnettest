using asp.Models;
using Microsoft.AspNetCore.Mvc;

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