using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.IO;
using System.Linq;

namespace FastSQL.MsAccess
{
    public class ConnectorAdapter : BaseAdapter
    {
        private IEnumerable<OptionItem> _options;
        protected override DbConnection GetConnection()
        {
            var builder = new ConnectionStringBuilder(_options);
            return new OdbcConnection(builder.Build());
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
            throw new NotImplementedException();
        }
    }
}
