using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace FastSQL.Core
{
    public interface IConnectorAdapter
    {
        IDbConnection GetConnection();
        IEnumerable<QueryResult> Query(string raw, object @params = null);
        int Execute(string raw, object @params = null);
    }
}
