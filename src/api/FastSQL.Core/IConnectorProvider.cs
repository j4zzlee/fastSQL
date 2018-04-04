using System;
using System.Collections.Generic;

namespace FastSQL.Core
{
    public interface IConnectorProvider
    {
        string Id { get; }
        string Name { get; }
        string DisplayName { get; }
        string Description { get; }
        IEnumerable<OptionItem> Options { get; }
        IConnectorProvider SetOptions(IEnumerable<OptionItem> options);
        bool TryConnect(out string message);
        IEnumerable<T> Query<T>(string rawQuery, object @params = null);
        int Execute(string rawQuery, object @params = null);
    }
}
