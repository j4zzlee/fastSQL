﻿using Dapper;
using FastSQL.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace FastSQL.MsSql
{
    public class ConnectorAdapter : BaseAdapter
    {
        private IEnumerable<OptionItem> _options;

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
                var dbName = _options.FirstOrDefault(o => o.Name == "Database")?.Value;
                var schema = conn.GetSchema("Views");
                return schema.Rows.Cast<DataRow>()
                    .Where(r => r["TABLE_CATALOG"].ToString() == dbName || string.IsNullOrWhiteSpace(dbName))
                    .Select(r => r["TABLE_NAME"].ToString());
            }
        }

        public override IConnectorAdapter SetOptions(IEnumerable<OptionItem> options)
        {
            _options = options;
            return this;
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

        protected override DbConnection GetConnection()
        {
            var builder = new ConnectionStringBuilder(_options);
            return new SqlConnection(builder.Build());
        }
    }
}
