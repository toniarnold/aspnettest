using asplib.Services;
using Microsoft.AspNetCore.Components;

namespace asplib.Components
{
    public class StaticComponentBase<T> : OwningComponentBase<T> where T : class, new()
    {
        [Inject]
        public T Main { get; private set; }

        /// <summary>
        /// Set the static reference to the injected main.
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                MainAccessor<T>.Instance = Main;
            }
        }
    }
}