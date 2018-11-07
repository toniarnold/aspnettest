using Microsoft.AspNetCore.Mvc;

namespace asp.Views.Calculator.Components.Error
{
    public class Error : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}