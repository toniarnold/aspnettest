using asplib.Model;
using asplib.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace iselenium
{
    /// <summary>
    /// Marked intterface for IEExtension database methods
    /// </summary>
    public interface IDatabase
    {
    }

    public static class DatabaseExtension
    {
        // Database maintenance
        internal static long max_mainid = long.MaxValue;    // guard against uninitialized WHERE mainid > @max_mainid

        /// <summary>
        /// [OneTimeSetUp]
        /// </summary>
        /// <param name="inst"></param>
        public static void SetUpDatabase(this IDatabase inst)
        {
            using (var db = new ASP_DBEntities())
            {
                var sql = @"
                    SELECT ISNULL(MAX(mainid), 0)
                    FROM Main
                    ";
                max_mainid = db.Database.SqlQuery<long>(sql).FirstOrDefault();
            }
        }

        /// <summary>
        /// [OneTimeTearDown]
        /// </summary>
        /// <param name="inst"></param>
        public static void TearDownDatabase(this IDatabase inst)
        {
            using (var db = new ASP_DBEntities())
            {
                var sql = @"
                    DELETE FROM Main
                    WHERE mainid > @max_mainid
                ";
                var param = new SqlParameter("max_mainid", max_mainid);
                db.Database.ExecuteSqlCommand(sql, param);
            }
        }
    }
}
