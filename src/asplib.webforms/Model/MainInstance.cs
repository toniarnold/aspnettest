using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace asplib.Model
{
    /// <summary>
    /// High level Db.Main model class providing support for persistence
    /// through a filter function (used for encryption) dealing with full Main
    /// object instances instead of just byte arrays from the table.
    /// </summary>
    public partial class Main
    {
        private object mainInstance;

        /// <summary>
        /// Returns all Database rows with a matching M instance which is already lazy loaded
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="filter">encrypt filter</param>

        /// <returns></returns>
        public static IEnumerable<Main> AllMainRows<M>(Func<byte[], byte[]> filter = null)
            where M : class
        {
            using (var db = new ASP_DBEntities())
            {
                return AllMainRows<M>(db, SerializationFilter.DecompressFilter(filter)).ToList();
            }
        }

        /// <summary>
        /// Testable AllMainRows
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="db">The database.</param>
        /// <param name="filter">encrypt filter</param>
        /// <returns></returns>
        public static IEnumerable<Main> AllMainRows<M>(ASP_DBEntities db, Func<byte[], byte[]> filter = null)
            where M : class
        {
            var all = from m in db.Main
                      select m;
            return from m in all.AsEnumerable()
                   where m.clsid == Clsid.Id(typeof(M)) &&
                         m.GetInstance<M>(filter) != null   // could be encrypted
                   select m;
        }

        /// <summary>
        /// Factory for fetching an M instance from the database.
        /// Applies the crypto filter if given.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="session">The session.</param>
        /// <param name="filter">encrypt filter</param>
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
        /// <typeparam name="M"></typeparam>
        /// <param name="db">The database.</param>
        /// <param name="session">The session.</param>
        /// <param name="filter">encrypt filter</param>
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
        /// Inserts or updates (when a row with the session exists) the M instance
        /// and returns the new session Guid if none is given or the row was not found.
        /// Applies the crypto filter if given.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="main">The main.</param>
        /// <param name="session">The session.</param>
        /// <param name="filter">encrypt filter</param>
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
        /// <param name="db">The database.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="session">The session.</param>
        /// <param name="filter">encrypt filter</param>
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
                main.clsid = Clsid.Id(instance);
                db.Main.Add(main);      // INSERT
            }
            main.SetInstance(instance, filter);
            db.SaveChanges();
            return main.session;  // get the new session guid set by the db on insert
        }

        /// <summary>
        /// Lazy Loads and returns a deserialized object instance from the
        /// [Main] table column byte[] main member or null if it is not of the generic type
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="filter">encrypt filter</param>
        /// <returns></returns>
        public M GetInstance<M>(Func<byte[], byte[]> filter = null)
            where M : class
        {
            var obj = Serialization.Deserialize(this.main, SerializationFilter.DecompressFilter(filter));
            this.mainInstance = (obj != null && obj.GetType() == typeof(M)) ? (M)obj : null;
            return (M)this.mainInstance;
        }

        /// <summary>
        /// Serializes the object instance to the [Main] table column byte[] main member
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="filter">encrypt filter</param>
        public void SetInstance<M>(M obj, Func<byte[], byte[]> filter = null)
            where M : class
        {
            this.mainInstance = obj;
            this.main = Serialization.Serialize(obj, filter);
        }

        /// <summary>
        /// Returns the (unencrypted!) literal INSERT string of the loaded object
        /// for manually exporting session dumps.
        /// </summary>
        /// <returns>
        /// SQL INSERT string
        /// </returns>
        public string InsertSQL()
        {
            Trace.Assert(this.main != null, "Explicit serialization with SetInstance(controlStorage.Main) required beforehand");
            var clsid = Clsid.Id(this.mainInstance);    // throws if no Clsid attribute present

            // Let the future consumer SQL Server encode the string representation of the byte[]
            string hex = String.Empty;
            var query = "SELECT CONVERT(VARCHAR(MAX), @main, 1) AS [hex]";
            using (var db = new ASP_DBEntities())
            {
                var param = new SqlParameter("main", this.main);
                hex = db.Database.SqlQuery<String>(query, param).FirstOrDefault();
            }
            // Format according to get copy-pasted into Management Studio
            return String.Format("INSERT INTO Main (clsid, main) SELECT '{0}', {1}\n" +
                                 "SELECT session FROM Main WHERE mainid = @@IDENTITY\n",
                                 clsid, hex);
        }
    }
}