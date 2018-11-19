using System;

namespace asplib.Model
{
    /// <summary>
    /// Declare a global unique identifier for object serialization.
    /// Gets assigned to Main.clsid.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class Clsid : Attribute
    {
        private Guid id;

        public Clsid(string guid)
        {
            this.id = Guid.Parse(guid);
        }

        public static Guid Id(object obj)
        {
            return Id(obj.GetType());
        }

        public static Guid Id(Type T)
        {
            var clsidAttributes = T.GetCustomAttributes(typeof(Clsid), false);
            if (clsidAttributes.Length == 0)
            {
                throw new ArgumentException(String.Format(
                    "{0}: missing Clsid(\"<guid>\") attribute", T.Name));
            }
            return ((Clsid)clsidAttributes[0]).id;
        }
    }
}