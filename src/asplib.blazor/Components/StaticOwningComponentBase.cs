using Microsoft.AspNetCore.Components;

namespace asplib.Components
{
    /// <summary>
    /// Testable Blazor OwningComponent which sets a static reference to its
    /// instance on TestFocus.Component and injects a public Main instance.
    /// (the owned service).
    /// </summary>
    public abstract class StaticOwningComponentBase<T> : OwningComponentBase<T>, IStaticComponent
        where T : class, new()
    {
        [Inject]
        public T Main { get; protected set; }

        /// <summary>
        /// Set the static reference to the component in focus on each Render,
        /// as the ElementReference.Id can change.
        /// Signals the TestFocus.Event if the component has focus.
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool _)
        {
            if (TestFocus.Expose(this))
            {
                TestFocus.Event.Set();  // allow the calling test method to continue
            }
        }
    }
}