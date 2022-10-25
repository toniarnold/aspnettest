using AngleSharp.Dom;
using asplib.Components;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace iselenium
{
    public static class BUnitRenderedFragmentExtensions
    {
        /// <summary>
        /// Synchronized click on the element: resets TestFocus.Event and waits for it.
        /// </summary>
        /// <param name="renderedFragment"></param>
        /// <param name="element"></param>
        /// <param name="expectRender"></param>
        /// <exception cref="TimeoutException"></exception>
        public static void Click(this IRenderedFragment renderedFragment, ElementReference element, int expectRenders = 1)
        {
            if (expectRenders > 0)
            {
                TestFocus.Event.Reset();    // defensive, should have been AutoReset
            }
            Find(renderedFragment, element).Click();
            while (expectRenders > 0)
            {
                if (!TestFocus.Event.WaitOne(SeleniumExtensionBase.RequestTimeout * 1000))
                {
                    throw new TimeoutException($"Click({element.Id}): TestFocus.Event not signaled");
                }
                expectRenders--;
            }
        }

        public static void Click(this IRenderedFragment renderedFragment, string cssSelector)
        {
            RenderedFragmentExtensions.Find(renderedFragment, cssSelector).Click();
        }

        /// <summary>
        /// Returns the first element for the @ref element instance
        /// </summary>
        /// <param name="renderedFragment">The rendered fragment to search.</param>
        /// <param name="element">The @ref element instance.</param>
        /// <returns></returns>
        public static IElement Find(this IRenderedFragment renderedFragment, ElementReference element)
        {
            return RenderedFragmentExtensions.Find(renderedFragment, RefCssSelector(element.Id));
        }

        /// <summary>
        /// Returns the first element for the @ref component instance
        /// </summary>
        /// <param name="renderedFragment">The rendered fragment to search.</param>
        /// <param name="component">The @ref component instance.</param>
        /// <returns></returns>
        public static IElement Find(this IRenderedFragment renderedFragment, InputCheckbox component)
        {
            return RenderedFragmentExtensions.Find(renderedFragment, RefCssSelector(component.Element?.Id));
        }

        /// <summary>
        /// Returns the first element for the @ref component instance
        /// </summary>
        /// <param name="renderedFragment">The rendered fragment to search.</param>
        /// <param name="component">The @ref component instance.</param>
        /// <returns></returns>
        public static IElement Find<T>(this IRenderedFragment renderedFragment, InputDate<T> component)
        {
            return RenderedFragmentExtensions.Find(renderedFragment, RefCssSelector(component.Element?.Id));
        }

        /// <summary>
        /// Returns the first element for the @ref component instance
        /// </summary>
        /// <param name="renderedFragment">The rendered fragment to search.</param>
        /// <param name="component">The @ref component instance.</param>
        /// <returns></returns>
        public static IElement Find(this IRenderedFragment renderedFragment, InputFile component)
        {
            return RenderedFragmentExtensions.Find(renderedFragment, RefCssSelector(component.Element?.Id));
        }

        /// <summary>
        /// Returns the first element for the @ref component instance
        /// </summary>
        /// <param name="renderedFragment">The rendered fragment to search.</param>
        /// <param name="component">The @ref component instance.</param>
        /// <returns></returns>

        public static IElement Find<T>(this IRenderedFragment renderedFragment, InputNumber<T> component)
        {
            return RenderedFragmentExtensions.Find(renderedFragment, RefCssSelector(component.Element?.Id));
        }

#if NET7_0_OR_GREATER
        /// <summary>
        /// Returns the first element for the @ref component instance
        /// </summary>
        /// <param name="renderedFragment">The rendered fragment to search.</param>
        /// <param name="component">The @ref component instance.</param>
        /// <returns></returns>
        public static IElement Find<T>(this IRenderedFragment renderedFragment, InputRadio<T> component)
        {
            return RenderedFragmentExtensions.Find(renderedFragment, RefCssSelector(component.Element?.Id));
        }
#endif

        /// <summary>
        /// Returns the first element for the @ref component instance
        /// </summary>
        /// <param name="renderedFragment">The rendered fragment to search.</param>
        /// <param name="component">The @ref component instance.</param>
        /// <returns></returns>
        public static IElement Find<T>(this IRenderedFragment renderedFragment, InputSelect<T> component)
        {
            return RenderedFragmentExtensions.Find(renderedFragment, RefCssSelector(component.Element?.Id));
        }

        /// <summary>
        /// Returns the first element for the @ref component instance
        /// </summary>
        /// <param name="renderedFragment">The rendered fragment to search.</param>
        /// <param name="component">The @ref component instance.</param>
        /// <returns></returns>
        public static IElement Find(this IRenderedFragment renderedFragment, InputText component)
        {
            return RenderedFragmentExtensions.Find(renderedFragment, RefCssSelector(component.Element?.Id));
        }

        /// <summary>
        /// Returns the first element for the @ref component instance
        /// </summary>
        /// <param name="renderedFragment">The rendered fragment to search.</param>
        /// <param name="component">The @ref component instance.</param>
        /// <returns></returns>
        public static IElement Find(this IRenderedFragment renderedFragment, InputTextArea component)
        {
            return RenderedFragmentExtensions.Find(renderedFragment, RefCssSelector(component.Element?.Id));
        }

        /// <summary>
        /// CSS selector for @ref elements with quoted blazor: namespace and the
        /// id in double quotes (this is bUnit-specific)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string RefCssSelector(string? id)
        {
            return $"[blazor\\:elementReference=\"{id ?? String.Empty}\"]";
        }
    }
}