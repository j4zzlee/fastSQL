using Dapper;
using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;

namespace FastSQL.MsAccess
{
    public class ConnectorAdapter : IConnectorAdapter
    {
        private string _filePath = string.Empty;
        public int Execute(string raw, object @params = null)
        {
            IDbConnection conn = null;
            try
            {
                using (conn = new OdbcConnection($"Driver={{Microsoft Access Driver (*.mdb, *.accdb)}};Dbq={_filePath};"))
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
                using (conn = new OdbcConnection($"Driver={{Microsoft Access Driver (*.mdb, *.accdb)}};Dbq={_filePath};"))
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

        internal ConnectorAdapter SetFilePath(string filePath)
        {
            _filePath = filePath;
            return this;
        }
    }
}
