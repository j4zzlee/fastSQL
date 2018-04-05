using Dapper;
using FastSQL.Core;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace FastSQL.MsSql
{
    public class ConnectorAdapter : BaseAdapter
    {
        private IEnumerable<OptionItem> _options;

        public override IDbConnection GetConnection()
        {
            var builder = new ConnectionStringBuilder(_options);
            return new SqlConnection(builder.Build());
        }

        internal ConnectorAdapter SetOptions(IEnumerable<OptionItem> selfOptions)
        {
            _options = selfOptions;
            return this;
        }
    }
}
