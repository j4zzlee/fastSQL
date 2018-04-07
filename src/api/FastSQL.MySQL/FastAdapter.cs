using FastSQL.Core;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace FastSQL.MySQL
{
    public class FastAdapter : BaseAdapter
    {
        public FastAdapter(FastProvider provider) : base(provider)
        {
        }

        protected override DbConnection GetConnection()
        {
            var builder = new ConnectionStringBuilder(Options);
            return new MySqlConnection(builder.Build());
        }

        public override bool TryConnect(out string message)
        {
            IDbConnection conn = null;
            try
            {
                message = "Connected.";
                using (conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
            finally
            {
                conn?.Dispose();
            }
        }

        public override IEnumerable<string> GetTables()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var schema = conn.GetSchema("Tables");
                return schema.Rows.Cast<DataRow>().Select(r => r["TABLE_NAME"].ToString());
            }
        }

        public override IEnumerable<string> GetViews()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var dbName = Options.FirstOrDefault(o => o.Name == "Database")?.Value;
                var schema = conn.GetSchema("Views");
                return schema.Rows.Cast<DataRow>()
                    .Where(r => r["TABLE_SCHEMA"].ToString() == dbName || string.IsNullOrWhiteSpace(dbName))
                    .Select(r => r["TABLE_NAME"].ToString());
            }
        }
    }
}
