using asplib.Controllers;
using Microsoft.AspNetCore.Mvc;
using minimal.Models;
using System;

namespace minimal.Controllers
{
    public class WithStaticController : Controller, IStaticController
    {
        // Any contained object to perform assertions on, the ViewModel is just
        // the most prominent one.
        public WithStaticViewModel Model { get; set; }

        public WithStaticController() : base()
        {
            this.SetController();
        }

        [HttpGet]
        public IActionResult Index(WithStaticViewModel model)
        {
            this.Model = model;
            return View(model);
        }

        [HttpPost]
        public IActionResult Submit(WithStaticViewModel model)
        {
            this.Model = model;
            // "Fighting the framework" by bypassing 1 pass model binding...
            model.ContentString = Request.Form["ContentString"];

            model.Content.Add(model.ContentTextBox);
            model.ContentTextBox = String.Empty; // never updated without RedirectToAction
            return View("Index", model);
        }
    }
}