using AngleSharp.Dom;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace iselenium
{
    public static class BUnitRenderedFragmentExtensions
    {
        public static IElement Find(this IRenderedFragment renderedFragment, ElementReference element)
        {
            return RenderedFragmentExtensions.Find(renderedFragment, RefCssSelector(element.Id));
        }

        public static IElement Find(this IRenderedFragment renderedFragment, InputCheckbox component)
        {
            return RenderedFragmentExtensions.Find(renderedFragment, RefCssSelector(component.Element?.Id));
        }

        public static IElement Find<T>(this IRenderedFragment renderedFragment, InputDate<T> component)
        {
            return RenderedFragmentExtensions.Find(renderedFragment, RefCssSelector(component.Element?.Id));
        }

        public static IElement Find(this IRenderedFragment renderedFragment, InputFile component)
        {
            return RenderedFragmentExtensions.Find(renderedFragment, RefCssSelector(component.Element?.Id));
        }

        public static IElement Find<T>(this IRenderedFragment renderedFragment, InputNumber<T> component)
        {
            return RenderedFragmentExtensions.Find(renderedFragment, RefCssSelector(component.Element?.Id));
        }

#if NET7_0_OR_GREATER
        public static IElement Find<T>(this IRenderedFragment renderedFragment, InputRadio<T> component)
        {
            return RenderedFragmentExtensions.Find(renderedFragment, RefCssSelector(component.Element?.Id));
        }
#endif

        public static IElement Find<T>(this IRenderedFragment renderedFragment, InputSelect<T> component)
        {
            return RenderedFragmentExtensions.Find(renderedFragment, RefCssSelector(component.Element?.Id));
        }

        public static IElement Find(this IRenderedFragment renderedFragment, InputText component)
        {
            return RenderedFragmentExtensions.Find(renderedFragment, RefCssSelector(component.Element?.Id));
        }

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