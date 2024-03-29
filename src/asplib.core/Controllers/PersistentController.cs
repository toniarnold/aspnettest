﻿using asplib.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace asplib.Controllers
{
    /// <summary>
    /// Controller inheriting from the non-serializable Mvc.Controller
    /// with Serialize/Deserialize methods just for the additional members.
    /// </summary>
    public class PersistentController : Controller, IPersistentController
    {
        public IConfiguration Configuration
        { get { return this.configuration; } }

        [NonSerialized]
        protected IConfiguration configuration = default!;

        public Storage? SessionStorage { get; set; }

        [NonSerialized]
        private object? model;

        public object? Model
        { get { return this.model; } }

        public Guid? Session { get; set; }

        public PersistentController()
        {
        }   // NUnit

        public PersistentController(IConfiguration configuration) : base()
        {
            this.configuration = configuration;
            this.SetController();
        }

        /// <summary>
        /// Serialize the controller shallowly.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [NonAction]
        public byte[] Serialize(Func<byte[], byte[]>? filter = null)
        {
            return Serialization.Serialize(this.GetValues(), filter);
        }

        /// <summary>
        /// Deserialize the already instantiated controller shallowly.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="filter"></param>
        [NonAction]
        public virtual void Deserialize(byte[] bytes, Func<byte[], byte[]>? filter = null)
        {
            var members = (Dictionary<string, object>)Serialization.Deserialize(bytes, filter);
            if (members != null)
            {
                this.SetValues(members);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            switch (this.GetStorage())
            {
                case Storage.ViewState:
                    this.SaveViewState();
                    break;

                case Storage.Session:
                    this.SaveSession();
                    break;

                case Storage.Database:
                    this.SaveDatabase();
                    break;

                case Storage.Header:
                    this.SaveHeader();
                    break;

                default:
                    throw new NotImplementedException(String.Format(
                        "Storage {0}", this.GetStorage()));
            }

            base.OnActionExecuted(context);
        }

        /// <summary>
        /// View method assigning the model to the controller instance for later
        /// retrieval in test assertions
        /// </summary>
        /// <param name="name"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public override ViewResult View(string? name, object? model)
        {
            this.model = model;
            return base.View(name, model);
        }

        /// <summary>
        /// View method assigning the model to the controller instance for later
        /// retrieval in test assertions
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override ViewResult View(object? model)
        {
            this.model = model;
            return base.View(model);
        }

        /// <summary>
        /// Assign all name:value pairs  to our fields.
        /// Silently ignores instance fields not available in the members
        /// dictionary.
        /// </summary>
        /// <param name="members"></param>
        internal void SetValues(Dictionary<string, object> members)
        {
            var fields = new List<FieldInfo>();
            this.GetFields(this.GetType(), fields);
            foreach (var field in fields)
            {
                if (members.ContainsKey(field.Name))
                {
                    field.SetValue(this, members[field.Name]);
                }
            }
        }

        /// <summary>
        /// Get all name:value pairs from the fields to serialize.
        /// </summary>
        /// <returns></returns>
        internal Dictionary<string, object?> GetValues()
        {
            var fields = new List<FieldInfo>();
            this.GetFields(this.GetType(), fields);
            var values = new Dictionary<string, object?>();
            foreach (var field in fields)
            {
                values.Add(field.Name, field.GetValue(this));
            }
            return values;
        }

        /// <summary>
        /// Recursively get all FieldInfo for the class to be serialized.
        /// </summary>
        /// <param name="members"></param>
        internal void GetFields(Type type, List<FieldInfo> members)
        {
            var allFields = type.GetFields(BindingFlags.DeclaredOnly |
                                           BindingFlags.Instance |
                                           BindingFlags.Public |
                                           BindingFlags.NonPublic);
            var serializalbeFields = allFields.Where(f =>
                 f.FieldType.IsSerializable &&
                 !f.Attributes.HasFlag(FieldAttributes.NotSerialized));
            members.AddRange(serializalbeFields);
            if (type != typeof(PersistentController)) // ceiling parent
            {
                this.GetFields(type.BaseType!, members);
            }
            else
            {
                return; // base case: don't attempt to serialize transient Mvc.Controller members
            }
        }
    }
}