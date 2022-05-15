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
        /// Set the static reference to the component in focus on each Render,
        /// as the ElementReference.Id can change.
        /// Signals the TestFocus.Event if the component has focus.
        /// </summary>
        /// <param name="focussableComponent"></param>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        public static async Task ExposeSetEventAsync(this ITestFocus focussableComponent, bool firstRender)
        {
            await Task.CompletedTask;
            ExposeSetEvent(focussableComponent, firstRender);
        }

        /// <summary>
        /// To be called in OnAfterRenderAsync(bool firstRender):
        /// Set the static reference to the component in focus on each Render,
        /// as the ElementReference.Id can change.
        /// Signals the TestFocus.Event if the component has focus.
        /// </summary>
        /// <param name="focussableComponent"></param>
        /// <param name="firstRender"></param>
        public static void ExposeSetEvent(this ITestFocus focussableComponent, bool firstRender)
        {
            lock (TestFocus.LockObj)
            {
                if (TestFocus.Expose(focussableComponent))
                {
                    if (firstRender && TestFocus.AwaitingFirstRender)
                    {
                        TestFocus.AwaitingFirstRender = false;
                    }
                    if (!TestFocus.AwaitingFirstRender)
                    {
                        TestFocus.Event.Set();  // allow the calling test method to continue
                    }
                }
            }
        }
    }
}