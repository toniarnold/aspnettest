using System.ComponentModel;

namespace asplib.Components
{
    /// <summary>
    /// Static class to obtain a reference to a Component which has currently
    /// obtained focus from another thread, the test runner.
    /// </summary>
    public static class TestFocus
    {
        private static object _lockObj = new object();  // there should be no concurrent tests running anyway...
        private static Type? _focussedCommponentType;
        private static TypeDescriptionProvider? _attributeProvider;

        public static object? Component { get; private set; }

        public static AutoResetEvent Event { get; private set; } = new AutoResetEvent(false);   // for Navigate()

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
        private class TestFocusAttribute : Attribute
        {
        }

        /// <summary>
        /// Assign the calling component to the Component field. Unconditionally
        /// call e.g. in the OnAfterRenderAsync event - the method will only
        /// take effect when the component was given focus by SetFocus().
        /// Returns true if the component has focus and was assigned.
        /// </summary>
        /// <param name="component"></param>
        public static bool Expose(object component)
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
            lock (_lockObj)
            {
                RemoveFocus();
                _focussedCommponentType = componentType;
                _attributeProvider = TypeDescriptor.AddAttributes(componentType, new TestFocusAttribute());
            }
        }

        /// <summary>
        /// To remove stalled references, call this at least in the
        /// [OneTimeTearDown] method of the executing test.
        /// </summary>
        public static void RemoveFocus()
        {
            lock (_lockObj)
            {
                if (_focussedCommponentType != null && _attributeProvider != null)
                {
                    TypeDescriptor.RemoveProvider(_attributeProvider, _focussedCommponentType);
                    _focussedCommponentType = null;
                    _attributeProvider = null;
                    Component = null;
                }
            }
        }

        /// <summary>
        /// True if the component type has the TestFocusAttribute.
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        internal static bool HasFocus(Type componentType)
        {
            return (from Attribute a in TypeDescriptor.GetAttributes(componentType)
                    where a is TestFocusAttribute
                    select a).Any();
        }
    }
}