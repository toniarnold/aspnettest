using OpenQA.Selenium;
using System;

namespace iselenium
{
    public static class IWebElementExtension
    {
        [Obsolete("Replaced by IWebElement.GetAttribute without flags")]
#pragma warning disable IDE0060 // Nicht verwendete Parameter entfernen
        public static string getAttribute(this IWebElement inst, string name, int flags = 0)
#pragma warning restore IDE0060 // Nicht verwendete Parameter entfernen
        {
            return inst.GetAttribute(name);
        }
    }
}