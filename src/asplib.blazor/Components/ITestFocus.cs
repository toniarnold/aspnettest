namespace asplib.Components
{
    /// <summary>
    /// Marker interface which reassures that OnAfterRenderAsync signals
    /// TestFocus.Event and exposes itself on TestFocus.Component by calling
    /// TestFocus.Expose(this);
    /// </summary>
    public interface ITestFocus
    { }

    public static class TestFocusExtension
    {
        /// <summary>
        /// To be called in OnAfterRenderAsync(bool firstRender):
        /// Sets the static reference to the component in focus on each Render,
        /// as the ElementReference.Id changes on reinstantiation.
        /// Signals the TestFocus.Event if the component has focus.
        /// </summary>
        /// <param name="focussableComponent"></param>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        public static async Task EndRenderAsync(this ITestFocus focussableComponent, bool firstRender)
        {
            await Task.CompletedTask;
            EndRender(focussableComponent, firstRender);
        }

        /// <summary>
        /// To be called in OnAfterRenderAsync(bool firstRender):
        /// Set the static reference to the component in focus on each Render,
        /// as the ElementReference.Id can change.
        /// Signals the TestFocus.Event if the component has focus.
        /// </summary>
        /// <param name="focussableComponent"></param>
        /// <param name="firstRender"></param>
        public static void EndRender(this ITestFocus focussableComponent, bool firstRender)
        {
            lock (TestFocus.LockObj)
            {
                if (TestFocus.Expose(focussableComponent))
                {
                    if (firstRender && TestFocus.AwaitingRerender)
                    {
                        TestFocus.AwaitingRerender = false;
                    }
                    if (!TestFocus.AwaitingRerender)
                    {
                        TestFocus.Event.Set();  // allow the calling test method to continue
                    }
                }
            }
        }

        /// <summary>
        /// True when the component is in focus (after SetFocus())
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static bool HasFocus(this ITestFocus focussableComponent)
        {
            return TestFocus.HasFocus(focussableComponent.GetType());
        }
    }
}