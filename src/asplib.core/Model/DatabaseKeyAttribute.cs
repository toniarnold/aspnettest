using System;
using Microsoft.AspNetCore.DataProtection;

namespace asplib.Model
{
    /// <summary>
    /// Dynamically added attribute for the database encryption key
    /// when the Cookie is no more available in Blazor
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DatabaseKeyAttribute : Attribute
    {
        private static IDataProtector _protector;
        private readonly byte[] _key;

        internal byte[] Key => _protector.Unprotect(_key);

        public DatabaseKeyAttribute(byte[] key)
        {
            if (_protector == null)
            {
                var provider = DataProtectionProvider.Create("asplib");
                _protector = provider.CreateProtector("DatabaseKeyAttribute");
            }
            _key = _protector.Protect(key);
        }
    }
}