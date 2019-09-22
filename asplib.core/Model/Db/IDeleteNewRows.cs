using System;
using System.Collections.Generic;
using System.Data.SqlClient;    // not Microsoft.Data.SqlClient for EF
using System.Text.RegularExpressions;

namespace asplib.Model.Db
{
    public interface IDeleteNewRows
    {
        List<(string tablename, string columnname, object maxid)> MaxIds { get; set; }
    }

    /// <summary>
    /// Read/Write the database with System.Data.SqlClient (raw ADO.NET), not
    /// with EF Core's DbContext.Database.GetDbConnection() from the aspservice
    /// project, as opening/closing it outside of the context seems to interfere
    /// with the framework, eventually loosing the connection string altogether.
    /// </summary>
    public static class DeleteNeNewRowsExtension
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "arguments filtered")]
        public static void SelectMaxIds(this IDeleteNewRows inst, string connecctionString, string tablename, string columnname)
        {
            if (!IsWord(tablename)) throw new ArgumentException($"invalid: [{tablename}]", "tablename");
            if (!IsWord(columnname)) throw new ArgumentException($"invalid: [{columnname}]", "columnname");

            if (inst.MaxIds == null)
            {
                inst.MaxIds = new List<(string tablename, string columnname, object maxid)>();
            }

            var sql = $"SELECT MAX([{columnname}]) FROM [{tablename}]";
            using (var conn = new SqlConnection(connecctionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                var maxid = cmd.ExecuteScalar();
                inst.MaxIds.Add((tablename, columnname, maxid));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "valuess from inst.MaxIds")]
        public static void DeleteMaxIdRows(this IDeleteNewRows inst, string connecctionString)
        {
            using (var conn = new SqlConnection(connecctionString))
            {
                conn.Open();
                foreach ((string tablename, string columnname, object maxid) in inst.MaxIds)
                {
                    var sql = $"DELETE FROM [{tablename}] WHERE [{columnname}] > @maxid";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("maxid", maxid);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        internal static bool IsWord(string name)
        {
            return Regex.IsMatch(name, @"^\w+$");
        }
    }
}