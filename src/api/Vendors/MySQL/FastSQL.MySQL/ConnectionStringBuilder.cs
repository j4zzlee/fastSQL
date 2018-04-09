using FastSQL.Core;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FastSQL.MySQL
{
    public class ConnectionStringBuilder
    {
        private IEnumerable<OptionItem> _selfOptions;

        public ConnectionStringBuilder(IEnumerable<OptionItem> selfOptions)
        {
            _selfOptions = selfOptions;
        }

        public string Build()
        {
            var builder = new MySqlConnectionStringBuilder();
            var port = _selfOptions.FirstOrDefault(o => o.Name == "Port")?.Value;
            var sslModel = _selfOptions.FirstOrDefault(o => o.Name == "SslMode")?.Value;
            builder.Server = _selfOptions.FirstOrDefault(o => o.Name == "Server")?.Value;
            builder.Port = uint.Parse(!string.IsNullOrWhiteSpace(port) ? port : "3306");
            builder.UserID = _selfOptions.FirstOrDefault(o => o.Name == "UserID")?.Value;
            builder.Password = _selfOptions.FirstOrDefault(o => o.Name == "Password")?.Value;
            builder.Database = _selfOptions.FirstOrDefault(o => o.Name == "Database")?.Value;
            builder.SslMode = string.IsNullOrWhiteSpace(sslModel) ? MySqlSslMode.None : (MySqlSslMode) Enum.Parse(typeof(MySqlSslMode), sslModel);
            return builder.ToString();
        }
    }
}
