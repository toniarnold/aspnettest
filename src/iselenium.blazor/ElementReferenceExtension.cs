using Microsoft.AspNetCore.Components;

namespace iselenium
{
    public static class ElementReferenceExtension
    {
        /// <summary>
        /// Blazor Server generated _bl_{Id}="" attribute
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string IdAttr(this ElementReference element)
        {
            return $"_bl_{element.Id}";
        }
    }
}