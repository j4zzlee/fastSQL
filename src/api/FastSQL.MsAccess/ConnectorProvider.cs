using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace FastSQL.MsAccess
{
    public class ConnectorProvider : IConnectorProvider
    {
        private readonly ConnectorOptions _options;
        private readonly ConnectorAdapter _adapter;

        public string Id => "1064aecb081027138f91e1e7e401a99239f8928d";
        public string Name => "MsAccess";

        public string DisplayName => "Microsoft Access";

        public string Description => "Microsoft Access";

        private IEnumerable<OptionItem> _selfOptions;
        public IEnumerable<OptionItem> Options => _selfOptions ?? _options?.GetOptions() ?? new List<OptionItem>();

        public ConnectorProvider(ConnectorOptions options, ConnectorAdapter adapter)
        {
            _options = options;
            _adapter = adapter;
        }

        public bool TryConnect(out string message)
        {
            IDbConnection conn = null;
            try
            {
                message = "Connected.";
                var filePath = _selfOptions?.FirstOrDefault(o => o.Name == "db_path")?.Value;
                if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                {
                    message = $"File {filePath} not found.";
                    return false;
                }
                using (conn = new OdbcConnection($"Driver={{Microsoft Access Driver (*.mdb, *.accdb)}};Dbq={filePath};"))
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

        public IConnectorProvider SetOptions(IEnumerable<OptionItem> options)
        {
            _selfOptions = options;
            return this;
        }

        public IEnumerable<T> Query<T>(string rawSQL, object @params = null)
        {
            return _adapter
                .SetFilePath(_selfOptions?.FirstOrDefault(o => o.Name == "db_path")?.Value)
                .Query<T>(rawSQL, @params);
        }

        public int Execute(string rawQuery, object @params = null)
        {
            return _adapter
                .SetFilePath(_selfOptions?.FirstOrDefault(o => o.Name == "db_path")?.Value)
                .Execute(rawQuery, @params);
        }
    }
}
