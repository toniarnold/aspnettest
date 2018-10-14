using System;
using System.Collections.Generic;
using System.Text;
using asplib.Model;

namespace asplib.Controllers
{
    /// <summary>
    /// Extension interface for a Controller to make it persistent across requests.
    /// </summary>
    public interface IStorageController : IStaticController
    {
        dynamic ViewBag { get; }
    }

    /// <summary>
    /// Extension implementation with storage dependency
    /// </summary>
    public static class ControlStorageExtension
    {
        /// <summary>
        /// StorageID-String unique to store/retrieve/clear Controller
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        public static string GetStorageID(this IStorageController inst)
        {
            return GetStorageID(inst.GetType().Name);
        }

        /// <summary>
        /// StorageID-String unique to store/retrieve/clear Controller
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static string GetStorageID(string typeName)
        {
            return String.Format("_CONTROLLER_{0}", typeName);
        }

        /// <summary>
        /// Hidden html input for posting the serialized Controller
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        public static string ViewStateInput(this IStorageController inst)
        {
            byte[] bytes;
            if (inst is SerializableController)
            {
                bytes = ((SerializableController)inst).Serialize(); // shallow
            }
            else
            {
                bytes = Serialization.Serialize(inst);  // POCO Controller
            }
            return string.Format("<input type='hidden' name='{0}' value='{1}'/>",
                            GetStorageID(inst),
                            Convert.ToBase64String(bytes));
        }

        /// <summary>
        /// Add the ViewState ipnut tho the ViewBag for rendering in the view
        /// </summary>
        /// <param name="inst"></param>
        public static void AddViewState(this IStorageController inst)
        {
            inst.ViewBag.ViewStateInput = ViewStateInput(inst);
        }
    }
}
