using System;

namespace asplib.Model
{
    /// <summary>
    /// Dynamically added attribute for the database Main.session value
    /// when the Cookie is no more available in Blazor
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DatabaseSessionAttribute : Attribute
    {
        internal Guid Session;

        public DatabaseSessionAttribute(Guid session)
        {
            this.Session = session;
        }
    }
}