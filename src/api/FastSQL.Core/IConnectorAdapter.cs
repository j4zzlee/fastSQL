using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace FastSQL.Core
{
    public interface IConnectorAdapter
    {
        IEnumerable<QueryResult> Query(string raw, object @params = null);
        int Execute(string raw, object @params = null);
        IEnumerable<string> GetTables();
        IEnumerable<string> GetViews();
        IConnectorAdapter SetOptions(IEnumerable<OptionItem> options);
        bool TryConnect(out string message);
    }
}
