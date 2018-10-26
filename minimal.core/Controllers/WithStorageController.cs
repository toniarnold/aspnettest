using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using asplib.Controllers;
using asplib.Model;
using minimal.Models;
using Microsoft.Extensions.Configuration;
using iie;


namespace minimal.Controllers
{
    public class WithStorageController : SerializableController
    {
        // Any contained object to perform assertions on, the ViewModel is just
        // the most prominent one.
        public new WithStorageViewModel Model { get { return (WithStorageViewModel)base.Model; } }

        public List<string> ContentList = new List<string>();

        public WithStorageController(IConfigurationRoot configuration) : base(configuration) { }

        [HttpGet]
        public IActionResult Index(WithStorageViewModel model)
        {
            // Update the model for an explicitly loaded session
            model.Content = this.ContentList;  
            if (this.SessionStorage != null)
            {
                model.Storage = (Storage)this.SessionStorage;
            }
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
            if (String.Compare(model.ContentTextBox, "except", true) == 0)
            {
                throw new TestException("Malicious Content Exception");
            }
            this.ContentList.Add(model.ContentTextBox);
            model.ContentTextBox = String.Empty; // never updated without RedirectToAction
            model.Content = this.ContentList;   // persistent object transiently assigned to the model
            return View("Index", model);
        }
    }
}