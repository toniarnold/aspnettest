using Microsoft.AspNetCore.Mvc;

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