using FastSQL.Core;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace FastSQL.MySQL
{
    public class ConnectorAdapter : BaseAdapter
    {
        private IEnumerable<OptionItem> _options;

        public override IDbConnection GetConnection()
        {
            var builder = new ConnectionStringBuilder(_options);
            return new MySqlConnection(builder.Build());
        }

        internal ConnectorAdapter SetOptions(IEnumerable<OptionItem> selfOptions)
        {
            _options = selfOptions;
            return this;
        }
    }
}
