using Dapper;
using FastSQL.Core;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;

namespace FastSQL.MySQL
{
    public class ConnectorAdapter : IConnectorAdapter
    {
        private IEnumerable<OptionItem> _options;
        public int Execute(string raw, object @params = null)
        {
            IDbConnection conn = null;
            try
            {
                var builder = new ConnectionStringBuilder(_options);
                using (conn = new MySqlConnection(builder.Build()))
                {
                    conn.Open();
                    return conn.Execute(raw, @params);
                }
            }
            finally
            {
                conn?.Dispose();
            }
        }

        public IEnumerable<T> Query<T>(string raw, object @params = null)
        {
            IDbConnection conn = null;
            try
            {
                var builder = new ConnectionStringBuilder(_options);
                using (conn = new MySqlConnection(builder.Build()))
                {
                    conn.Open();
                    return conn.Query<T>(raw, @params);
                }
            }
            finally
            {
                conn?.Dispose();
            }
        }
        
        internal ConnectorAdapter SetOptions(IEnumerable<OptionItem> selfOptions)
        {
            _options = selfOptions;
            return this;
        }
    }
}
