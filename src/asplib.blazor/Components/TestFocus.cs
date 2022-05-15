namespace asplib.Components
{
    /// <summary>
    /// Static class to obtain a reference to a Component which has currently
    /// obtained focus from another thread, the test runner.
    /// </summary>
    public static class TestFocus
    {
        private static Type? _focussedCommponentType;

        public static ITestFocus Component { get; private set; } = default!;
        public static AutoResetEvent Event { get; private set; } = new(false);
        internal static object LockObj { get; private set; } = new object();

        /// <summary>
        /// Set to true when a new page load is expected such that the test
        /// waits for the firstRender OnAfterRenderAsync instead of continuing
        /// still on the page that caused the reload.
        /// </summary>
        public static bool AwaitingFirstRender { get; set; } = false;

        /// <summary>
        /// Assign the calling component to the Component field. Unconditionally
        /// call e.g. in the OnAfterRenderAsync event - the method will only
        /// take effect when the component was given focus by SetFocus().
        /// Returns true if the component has focus and was assigned.
        /// </summary>
        /// <param name="component"></param>
        public static bool Expose(ITestFocus component)
        {
            if (HasFocus(component.GetType()))
            {
                Component = component;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Set the test focus on the given componentType argument. The
        /// instances of the type of the current component should Expose()
        /// themselves e.g. in the OnAfterRenderAsync event. If the type
        /// matches, the component can be accessed in the Component attribute.
        /// Only one component can have focus at a time, therefore the method
        /// first removes the possible focus from another type.
        /// </summary>
        /// <param name="componentType"></param>
        public static void SetFocus(Type componentType)
        {
            if (!typeof(ITestFocus).IsAssignableFrom(componentType))
            {
                throw new ArgumentException($"The componentType {componentType} must implement IStaticComponent");
            }
            lock (LockObj)
            {
                RemoveFocus();
                _focussedCommponentType = componentType;
            }
        }

        /// <summary>
        /// To remove stalled references, call this at least in the
        /// [OneTimeTearDown] method of the executing test.
        /// </summary>
        public static void RemoveFocus()
        {
            lock (LockObj)
            {
                _focussedCommponentType = null;
                AwaitingFirstRender = false;
                Component = default!;
            }
        }

        /// <summary>
        /// True if the component type has the TestFocusAttribute.
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        internal static bool HasFocus(Type componentType)
        {
            return componentType.Equals(_focussedCommponentType);
        }
    }
}