namespace asplib.View
{
    /// <summary>
    /// Extension interface for a Control to access the SetRoot() method
    /// </summary>
    public interface IRootControl
    {
    }

    /// <summary>
    /// Extension implementation with minimal dependencies
    /// </summary>
    public static class ControlRootExtension
    {
        /// <summary>
        /// Global reference to the root control of an application under test
        /// </summary>
        private static System.Web.UI.Control RootControl { get; set; }

        /// <summary>
        /// Set once the global reference to the root control of an application under test
        /// </summary>
        /// <param name="controlRoot"></param>
        public static void SetRoot(this IRootControl controlRoot)
        {
            ControlRootExtension.RootControl = (System.Web.UI.Control)controlRoot;
        }

        /// <summary>
        /// Get the global reference to the root control of an application under test within tests
        /// </summary>
        /// <returns></returns>
        public static System.Web.UI.Control GetRoot()
        {
            return ControlRootExtension.RootControl;
        }

        /// <summary>
        /// Delete the global RootControl reference
        /// </summary>
        public static void TearDownRoot()
        {
            ControlRootExtension.RootControl = null;
        }
    }
}