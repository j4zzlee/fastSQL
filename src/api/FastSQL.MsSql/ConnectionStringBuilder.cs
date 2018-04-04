using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace FastSQL.MsSql
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
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = _selfOptions.FirstOrDefault(o => o.Name == "DataSource").Value;
            builder.UserID = _selfOptions.FirstOrDefault(o => o.Name == "UserID").Value;
            builder.Password = _selfOptions.FirstOrDefault(o => o.Name == "Password").Value;
            return builder.ToString();
        }
    }
}
