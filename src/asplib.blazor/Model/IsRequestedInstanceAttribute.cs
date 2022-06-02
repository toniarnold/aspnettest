using System.ComponentModel;

namespace asplib.Model
{
    /// <summary>
    /// Dynamically added attribute to mark an instance as requested by GET
    /// ?session= to block overwriting it by other storage mechanisms.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IsRequestedInstanceAttribute : Attribute
    {
    }
}