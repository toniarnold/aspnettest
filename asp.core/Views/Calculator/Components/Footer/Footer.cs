using Microsoft.AspNetCore.Mvc;

namespace asp.Views.Calculator.Components.Footer
{
    public class Footer : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}