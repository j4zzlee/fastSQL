using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.Sync.Core.Settings
{
    public interface ISettingProvider: IDisposable
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        bool Optional { get; }
        string Message { get; set; }
        Task<bool> Validate();
        ISettingProvider Save();
        IEnumerable<string> Commands { get; }
        Task<bool> Invoke(string commandName);
        IEnumerable<OptionItem> Options { get; }

        ISettingProvider SetOptions(IEnumerable<OptionItem> enumerable);
    }
}
