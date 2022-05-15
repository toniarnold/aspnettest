using Microsoft.AspNetCore.Components;

namespace asplib.Components
{
    /// <summary>
    /// Testable Blazor OwningComponent which sets a static reference to its
    /// instance on TestFocus.Component and injects a public Main instance.
    /// (the owned service).
    /// </summary>
    public abstract class StaticOwningComponentBase<T> : OwningComponentBase<T>, ITestFocus
        where T : class, new()
    {
        [Inject]
        public T Main { get; protected set; } = default!;

        /// <summary>
        /// Set the static reference to the component in focus on each Render,
        /// as the ElementReference.Id can change.
        /// Signals the TestFocus.Event if the component has focus.
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await this.ExposeSetEventAsync(firstRender);
        }
    }
}