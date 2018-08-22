using System;
using System.Collections.Generic;

namespace FastSQL.Core
{
    public interface IOptionManager: IDisposable
    {
        IOptionManager SetOptions(IEnumerable<OptionItem> options);
        IEnumerable<OptionItem> Options { get; }
        IEnumerable<OptionItem> GetOptionsTemplate();
    }
}
