using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using asplib.Controllers;
using minimal.Models;
using Microsoft.Extensions.Configuration;


namespace minimal.Controllers
{
    public class WithStorageController : SerializableController
    {
        private List<string> ContentList = new List<string>();

        public WithStorageController(IConfigurationRoot configuration) : base(configuration) { }

        [HttpGet]
        public IActionResult Index(WithStorageViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult ChangeStorage(WithStorageViewModel model)
        {
            this.SessionStorage = model.Storage;
            model.Content = this.ContentList;
            return View("Index", model);
        }

        [HttpPost]
        public IActionResult Submit(WithStorageViewModel model) 
        {
            this.SessionStorage = model.Storage;
            this.ContentList.Add(model.ContentTextBox);
            model.ContentTextBox = String.Empty; // never updated without RedirectToAction
            model.Content = this.ContentList;   // persistent object transiently assigned to the model
            return View("Index", model);
        }
    }
}