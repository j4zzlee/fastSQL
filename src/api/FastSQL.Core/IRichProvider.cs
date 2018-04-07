using System;
using System.Collections.Generic;

namespace FastSQL.Core
{
    public interface IRichProvider
    {
        string Id { get; }
        string Name { get; }
        string DisplayName { get; }
        string Description { get; }
        IEnumerable<OptionItem> Options { get; }
        IRichProvider SetOptions(IEnumerable<OptionItem> options);
    }
}
