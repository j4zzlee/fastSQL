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
            var username = _selfOptions.FirstOrDefault(o => o.Name == "UserID").Value;
            var password = _selfOptions.FirstOrDefault(o => o.Name == "Password").Value;
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = _selfOptions.FirstOrDefault(o => o.Name == "DataSource").Value,
                MultipleActiveResultSets = true,
                //builder.MultiSubnetFailover = true;
                Pooling = true,
                InitialCatalog = _selfOptions.FirstOrDefault(o => o.Name == "Database").Value
            };
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                builder.UserID = username;
                builder.Password = password;
                builder.IntegratedSecurity = false;
            }
            else
            {
                builder.IntegratedSecurity = true;
            }
            
            return builder.ToString();
        }
    }
}
