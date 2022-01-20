using System;

namespace asplib.Model
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple =false)]
    public class DatabaseKeyAttribute : Attribute
    {
        internal byte[] Key;

        public DatabaseKeyAttribute(byte[] key)
        {
            this.Key = key;
        }
    }
}