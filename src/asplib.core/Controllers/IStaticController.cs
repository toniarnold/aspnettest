using System;

namespace asplib.Controllers
{
    /// <summary>
    /// Extension interface for a Controller to access the SetController() method
    /// </summary>
    public interface IStaticController
    {
    }

    /// <summary>
    /// Extension implementation with minimal dependencies
    /// </summary>
    public static class StaticControllerExtension
    {
        /// <summary>
        /// Global reference to the static controller of an application under test
        /// </summary>
        private static object? Controller { get; set; }

        /// <summary>
        /// Set once the global reference to controller of an application under test.
        /// The model must be assigned in each individual action method.
        /// </summary>
        /// <param name="control"></param>
        public static void SetController(this object controller)
        {
            Controller = controller;
        }

        /// <summary>
        /// Get the global reference to the root control of an application under test within tests
        /// </summary>
        /// <returns></returns>
        public static object GetController()
        {
            if (Controller == null)
            {
                throw new NullReferenceException("Controller not set or torn down");
            }
            return Controller;
        }

        /// <summary>
        /// Delete the global controller reference
        /// </summary>
        public static void TearDownController()
        {
            Controller = null;
        }
    }
}