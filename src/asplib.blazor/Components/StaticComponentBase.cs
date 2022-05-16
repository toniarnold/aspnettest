using Microsoft.AspNetCore.Components;

namespace asplib.Components
{
    /// <summary>
    /// Testable Blazor Component which sets a static reference to its instance
    /// on TestFocus.Component.
    /// </summary>
    public class StaticComponentBase : ComponentBase, ITestFocus

    {
        /// <summary>
        /// Set the static reference to the component in focus on each Render,
        /// as the ElementReference.Id can change.
        /// Signals the TestFocus.Event if the component has focus.
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await this.EndRenderAsync(firstRender);
        }
    }
}