using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Core
{
    public interface IConnectorAdapter
    {
        IEnumerable<T> Query<T>(string raw, object @params = null);
        int Execute(string raw, object @params = null);
    }
}
