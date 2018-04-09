using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core
{
    public interface IVendor
    {
        string Id { get; }
        string Name { get; }
        string DisplayName { get; }
        string Description { get; }
        IEnumerable<OptionItem> Options { get; }
        IVendor SetOptions(IEnumerable<OptionItem> options);
    }
}
