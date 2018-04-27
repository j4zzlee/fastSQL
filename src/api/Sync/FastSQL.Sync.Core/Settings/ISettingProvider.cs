using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Settings
{
    public interface ISettingProvider
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        bool Optional { get; }
        bool Validate(out string message);
        ISettingProvider Save();
        IEnumerable<string> Commands { get; }
        bool Invoke(string commandName, out string message);
        IEnumerable<OptionItem> Options { get; }

        ISettingProvider SetOptions(IEnumerable<OptionItem> enumerable);
    }
}
