using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace asplib.Model
{
    /// <summary>
    /// High level Db.Main model class providing support for persistence through a filter function
    /// used for encryption.
    /// </summary>
    public partial class Main
    {
        private object mainInstance;

        /// <summary>
        /// Returns all Database rows with a matching M instance which is already lazy loaded
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
        /// Lazy Loads and returns a deserialized object instance from the
        /// [Main]  table column byte[] main member or null if it is not of the generic type
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
        /// Serializes the object instance to the [Main] table column byte[] main member
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="obj"></param>
        public void SetInstance<M>(M obj, Func<byte[], byte[]> filter = null)
            where M : class
        {
            this.mainInstance = obj;
            this.main = this.Serialize(obj, filter);
        }

        /// <summary>
        /// Returns the (unencrypted!) literal INSERT string of the loaded object
        /// for manually exporting session dumps.
        /// </summary>
        /// <returns>SQL INSERT string</returns>
        public string InsertSQL()
        {
            Trace.Assert(this.main != null, "Explicit serialization with SetInstance(controlStorage.Main) required beforehand");

            // Let the future consumer SQL Server encode the string representation of the byte[]
            string hex = String.Empty;
            var query = "SELECT CONVERT(VARCHAR(MAX), @main, 1) AS [hex]";
            using (var db = new ASP_DBEntities())
            {
                var param = new SqlParameter("main", this.main);
                hex = db.Database.SqlQuery<String>(query, param).FirstOrDefault();
            }
            // Format according to get copy-pasted into Management Studio
            return String.Format("INSERT INTO Main (main) SELECT {0}\n" +
                                 "SELECT session FROM Main WHERE mainid = @@IDENTITY\n",
                                 hex);
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
        /// Returns null if the deserialization fails for whatever reason (wrong key, old version...)
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
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
                catch
                {
                    return null;
                }
            }
        }
    }
}