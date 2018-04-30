using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core
{
    public interface ITransformer
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        ITransformer SetOptions(IEnumerable<OptionItem> options);
        IEnumerable<OptionItem> Options { get; }
        IEnumerable<OptionItem> GetOptionsTemplate();

        object Transform(object value);
        T Transaform<T>(object value);
    }
}
