﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace asplib.Model
{
    public partial class Main
    {
        private object mainInstance;

        /// <summary>
        /// Returns all Database rows with a matching R instance which is already lazy loaded
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <returns></returns>
        public static IEnumerable<Main> AllMainRows<M>(Func<byte[], byte[]> filter = null)
            where M : class
        {
            using (var db = new ASP_DBEntities())
            {
                return AllMainRows<M>(db, filter).ToList(); // enumerate within the DbContext
            }   
        }

        /// <summary>
        /// Testable AllMainRows
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <returns></returns>
        public static IEnumerable<Main> AllMainRows<M>(ASP_DBEntities db, Func<byte[], byte[]> filter = null)
            where M : class
        {
            // Fetching all rows from the database and then restricting the result to the correct
            // types is of course inefficient for big tables and many types - an optimization would
            // be to explicitly store the type name in a [Type] table and reference them in [Main]
            // to be able to restrict the SELECT directly.
            var all = from m in db.Main
                      select m;
            return from m in all.AsEnumerable()
                    where m.GetInstance<M>(filter) != null
                    select m;
        }


        /// <summary>
        /// Factory for fetching a R instance from the database
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="session"></param>
        /// <returns></returns>
        public static M LoadMain<M>(Guid session, Func<byte[], byte[]> filter = null)
            where M : class
        {
            using (var db = new ASP_DBEntities())
            {
                return LoadMain<M>(db, session, filter);
            }
        }

        /// <summary>
        /// Testable LoadMain
        /// </summary>
        /// <param name="db"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        internal static M LoadMain<M>(ASP_DBEntities db, Guid session, Func<byte[], byte[]> filter = null)
            where M : class
        {
            var query = from m in db.Main
                        where m.session == session
                        select m;
            var main = query.FirstOrDefault();
            return (main != null) ? main.GetInstance<M>(filter) : null;
        }

        /// <summary>
        /// Inserts or updates (when a row with the session exists) the R instance
        /// and returns the new session Guid if none is given or the row was not found
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="main"></param>
        /// <returns></returns>
        public static Guid SaveMain<M>(M main, Guid? session, Func<byte[], byte[]> filter = null)
            where M : class
        {
            using (var db = new ASP_DBEntities())
            {
                return SaveMain<M>(db, main, session, filter);
            }
        }

        /// <summary>
        /// Testable SaveMain
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="main"></param>
        /// <returns></returns>
        public static Guid SaveMain<M>(ASP_DBEntities db, M instance, Guid? session, Func<byte[], byte[]> filter = null)
            where M : class
        {
            var query = from m in db.Main
                        where m.session == session
                        select m;
            var main = query.FirstOrDefault();
            if (main == null)
            {
                main = new Main();
                db.Main.Add(main);      // INSERT
            }
            main.SetInstance(instance, filter);
            db.SaveChanges();
            return main.session;  // get the new session guid set by the db on insert
        }


        /// <summary>
        /// Lazy Loads and returns a deserialized object instance from the column byte[] main
        /// or null if it is not of the generic type
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <returns></returns>
        public M GetInstance<M>(Func<byte[], byte[]> filter = null)
            where M : class
        {
            var obj = this.Deserialize(this.main, filter);
            this.mainInstance = (obj != null && obj.GetType() == typeof(M)) ? (M)obj : null;
            return (M)this.mainInstance;
        }

        /// <summary>
        /// Writes the serialized object to the the column byte[] main and to the instance.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="obj"></param>
        public void SetInstance<M>(M obj, Func<byte[], byte[]> filter = null)
        {
            this.mainInstance = obj;
            this.main = this.Serialize(obj, filter);
        }


        /// <summary>
        /// Serializes any object into a byte array and apply the crypto filter if given
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal byte[] Serialize(object obj, Func<byte[], byte[]> filter = null)
        {
            using (var stream = new MemoryStream())
            {
                var formattter = new BinaryFormatter();
                formattter.Serialize(stream, obj);
                return (filter == null) ? stream.ToArray() : filter(stream.ToArray());
            }
        }

        /// <summary>
        /// Deserializes a byte array into an object and apply the crypto filter if given
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        internal object Deserialize(byte[] bytes, Func<byte[], byte[]> filter = null)
        {
            using (var stream = new MemoryStream((filter == null) ? bytes : filter(bytes)))
            using (var writer = new BinaryWriter(stream))
            {
                var formattter = new BinaryFormatter();
                try
                {
                    return formattter.Deserialize(stream);
                }
                catch (SerializationException)
                {
                    return null;
                }
            }
        }
    }
}
