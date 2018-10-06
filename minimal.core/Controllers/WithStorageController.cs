using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace minimal.core.Controllers
{
    public class WithStorageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}