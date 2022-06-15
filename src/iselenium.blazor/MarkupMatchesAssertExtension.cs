using Bunit.Asserting;
using OpenQA.Selenium;

namespace iselenium
{
    public static class MarkupMatchesAssertExtension
    {
        /// <summary>
        /// Semantic HTML comparer from bUnit, see https://bunit.dev/docs/verification/verify-markup.html
        /// </summary>
        /// <param name="element"></param>
        /// <param name="expected"></param>
        /// <param name="userMessage"></param>
        [AssertionMethod]
        public static void MarkupMatches(this IWebElement element, string expected, string? userMessage = null)
        {
            var outerHTML = element.GetAttribute("outerHTML");
            Bunit.MarkupMatchesAssertExtensions.MarkupMatches(outerHTML, expected, userMessage);
        }
    }
}