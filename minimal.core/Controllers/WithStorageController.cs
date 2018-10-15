using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using asplib.Controllers;
using minimal.Models;

namespace minimal.Controllers
{
    public class WithStorageController : SerializableController
    {
        private List<string> ContentList = new List<string>();

        public WithStorageController() : base()
        {
            this.SetController();
        }

        [HttpGet]
        public IActionResult Index(WithStorageViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult Submit(WithStorageViewModel model) 
        {
            this.ContentList.Add(model.ContentTextBox);
            model.ContentTextBox = String.Empty; // never updated without RedirectToAction
            model.Content = this.ContentList;   // persistent object transiently assigned to the model
            return View("Index", model);
        }
    }
}