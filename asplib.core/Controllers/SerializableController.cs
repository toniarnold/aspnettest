using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Runtime.CompilerServices;
using asplib.Model;
using Microsoft.AspNetCore.Mvc.Filters;

[assembly: InternalsVisibleTo("test.core")]
namespace asplib.Controllers
{
    /// <summary>
    /// Controller inheriting from the non-serializable Mvc.Controller
    /// with Serialize/Deserialize methods just for the additional members.
    /// </summary>
    public class SerializableController : Controller, IStorageController
    {
        /// <summary>
        /// Serialize the controller shallowly.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public byte[] Serialize(Func<byte[], byte[]> filter = null)
        {
            return Serialization.Serialize(this.GetValues(), filter);
        }


        /// <summary>
        /// Deserialize the already instantiated controller shallowly.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="filter"></param>
        public void Deserialize(byte[] bytes, Func<byte[], byte[]> filter = null)
        {
            var members = (Dictionary<string, object>)Serialization.Deserialize(bytes, filter);
            this.SetValues(members);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            this.AddViewState();
            base.OnActionExecuted(context);
        }

        /// <summary>
        /// Assign  all name:value pairs  to our fields
        /// </summary>
        /// <param name="members"></param>
        internal void SetValues(Dictionary<string, object> members)
        {
            var fields = new List<FieldInfo>();
            this.GetFields(this.GetType(), fields);
            foreach (var field in fields)
            {
                field.SetValue(this, members[field.Name]);
            }
        }


        /// <summary>
        /// Get all name:value pairs from the fields to serialize.
        /// </summary>
        /// <returns></returns>
        internal Dictionary<string, object> GetValues()
        {
            var fields = new List<FieldInfo>();
            this.GetFields(this.GetType(), fields);
            var values = new Dictionary<string, object>();
            foreach (var field in fields)
            {
                values.Add(field.Name, field.GetValue(this));
            }
            return values;
        }


        /// <summary>
        /// Recursively get all FieldInfo for the class.
        /// </summary>
        /// <param name="members"></param>
        internal void GetFields(Type type, List<FieldInfo> members)
        {
            members.AddRange(type.GetFields(BindingFlags.DeclaredOnly |
                                            BindingFlags.Instance |
                                            BindingFlags.Public |
                                            BindingFlags.NonPublic));
            if (type != typeof(SerializableController))
            {
                this.GetFields(type.BaseType, members);
            }
            else
            {
                return; // base case: don't attempt to serialize transient Mvc.Controller members

            }
        }
    }
}
