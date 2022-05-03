using System;

namespace asplib.Model
{
    /// <summary>
    /// Dynamically added attribute for the database encryption key
    /// when the Cookie is no more available in Blazor
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DatabaseKeyAttribute : Attribute
    {
        internal byte[] Key;

        public DatabaseKeyAttribute(byte[] key)
        {
            this.Key = key;
        }
    }
}