using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace FastSQL.Core
{
    public interface ISqlAdapter: IRichAdapter
    {
        IEnumerable<QueryResult> Query(string raw, object @params = null);
        int Execute(string raw, object @params = null);

        IEnumerable<string> GetTables();
        IEnumerable<string> GetViews();
    }
    public interface IRichAdapter: IOptionManager
    {
        IRichProvider GetProvider();
        bool IsProvider(string providerId);
        bool IsProvider(IRichProvider provider);
        bool TryConnect(out string message);
    }
}
