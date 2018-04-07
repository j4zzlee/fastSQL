using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace FastSQL.Core
{
    public interface IRichAdapter
    {
        IRichProvider GetProvider();
        IRichAdapter SetOptions(IEnumerable<OptionItem> options);
        bool TryConnect(out string message);

        IEnumerable<QueryResult> Query(string raw, object @params = null);
        int Execute(string raw, object @params = null);

        IEnumerable<string> GetTables();
        IEnumerable<string> GetViews();
    }
}
