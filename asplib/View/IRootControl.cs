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
        public static System.Web.UI.Control RootControl { get; set; }

        public static void SetRoot(this IRootControl controlRoot)
        {
            ControlRootExtension.RootControl = (System.Web.UI.Control)controlRoot;
        }
    }
}