using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace asplib.Model.Db
{
    /// <summary>
    /// ASP.NET Core Entity Framework DAL
    /// </summary>
    public class ASP_DBEntities : DbContext
    {
        public static string ConnectionString = "";

        public DbSet<Main> Main { get; set; }

        public ASP_DBEntities() : base()
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => { builder.AddConsole(); }))
                .EnableSensitiveDataLogging()
                .UseSqlServer(ConnectionString);
        }

        /// <summary>
        /// Factory for fetching a  byte array from the database
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public byte[] LoadMain(Guid session)
        {
            var query = from m in this.Main
                        where m.session == session
                        select m;
            var main = query.FirstOrDefault();
            return (main != null) ? main.main : null;
        }

        /// <summary>
        /// Inserts or updates (when a row with the session exists) the byte array
        /// and returns the new session Guid if none is given or the row was not found
        /// </summary>
        /// <param name="type"></param>
        /// <param name="bytes"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public Guid SaveMain(Type type, byte[] bytes, Guid? session)
        {
            var query = from m in this.Main
                        where m.session == session
                        select m;
            var main = query.FirstOrDefault();
            if (main == null)
            {
                main = new Main();
                this.Main.Add(main);    // INSERT
            }
            main.clsid = Clsid.Id(type);
            main.main = bytes;
            this.SaveChanges();
            return main.session;  // get the new session guid set by the db on insert
        }

        /// <summary>
        /// Returns the (unencrypted!) literal INSERT string of the loaded object
        /// for manually exporting session dumps.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="bytes"></param>
        /// <returns>SQL INSERT string</returns>
        public string InsertSQL(Type type, byte[] bytes)
        {
            var clsid = Clsid.Id(type);
            // Let the future consumer SQL Server encode the string
            // representation of the byte[] Unlike EF6 use ADO.NET Core, as the
            // connection string is usable for both contexts.
            string hex = String.Empty;
            var query = "SELECT CONVERT(VARCHAR(MAX), @main, 1) AS [hex]";
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("main", bytes);
                hex = (string)cmd.ExecuteScalar();
            }
            // Format according to get copy-pasted into Management Studio
            return String.Format("INSERT INTO Main (clsid, main) SELECT '{0}', {1}\n" +
                                 "SELECT session FROM Main WHERE mainid = @@IDENTITY\n",
                                 clsid, hex);
        }
    }
}