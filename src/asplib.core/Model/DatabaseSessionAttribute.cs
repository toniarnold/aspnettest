using System;

namespace asplib.Model
{
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