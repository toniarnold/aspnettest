using asplib.Services;
using Microsoft.AspNetCore.Components;

namespace asplib.Components
{
    public class StaticComponentBase<T> : OwningComponentBase<T> where T : class, new()
    {
        [Inject]
        public T Main { get; protected set; }

        /// <summary>
        /// Set the static reference to the injected main on each Render,
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