using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Linq;

namespace asplib.Model.Db
{
    /// <summary>
    /// Custom extensions for the auto-generated ASP.NET Core Entity Framework DAL
    /// </summary>
    public partial class ASP_DBEntities : DbContext
    {
        public static string ConnectionString = "";
        private static IServiceProvider? ServiceProvider;

        #region amend not auto-generated features

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Main>(entity =>
            {
                entity.Property(e => e.Changed)
                     .ValueGeneratedOnAddOrUpdate();    // updated by TRG_Main_changed
            });
        }

        #endregion amend not auto-generated features

        #region ServiceProviderCache workaround

        /// <summary>
        /// Add a static internal ServiceProvider for using the ASP_DBEntities
        /// DBContext in VS Test-Explorer tests ("insulated" from a running
        /// ASP.NET Core instance, without functional static
        /// ServiceProviderCache).
        /// </summary>
        /// <param name="connectionString"></param>
        public static void SetUpInsulatedDbContext(string connectionString)
        {
            ConnectionString = connectionString;

            // See https://github.com/toniarnold/aspnettest/issues/5 A Logger is
            // required, but cannot be used when
            // Microsoft.AspNetCore.Components.Web is referenced (through
            // asplib.blazor), but the NullLoggerFactory works within tests
            // (without the full ASP.NET Core context).
            var sc = new ServiceCollection();
            sc.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);
            sc.AddEntityFrameworkSqlServer();
            var sp = sc.BuildServiceProvider();
            ASP_DBEntities.ServiceProvider = sp;
        }

        /// <summary>
        /// Remove the global static internal service provider and the
        /// connection string.
        /// </summary>
        public static void TearDownInsulatedDbContext()
        {
            ConnectionString = String.Empty; ;
            ServiceProvider = null;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (ServiceProvider != null)
            {
                optionsBuilder.UseInternalServiceProvider(ServiceProvider);
            }
            optionsBuilder.UseSqlServer(ConnectionString);
        }

        #endregion ServiceProviderCache workaround

        #region queries

        /// <summary>
        /// Factory for fetching a  byte array from the database
        /// </summary>
        /// <param name="session"></param>
        /// <returns>a byte array, else null if not found</returns>
        public byte[]? LoadMain(Guid session)
        {
            var query = from m in this.Main
                        where m.Session == session
                        select m;
            var main = query.FirstOrDefault();
            return (main != null) ? main.Main1 : null;
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
                        where m.Session == session
                        select m;
            var main = query.FirstOrDefault();
            if (main == null)
            {
                main = new Main();
                this.Main.Add(main);    // INSERT
            }
            main.Clsid = Clsid.Id(type);
            main.Main1 = bytes;
            this.SaveChanges();
            return main.Session;  // get the new session guid set by the db on insert
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

        #endregion queries
    }
}