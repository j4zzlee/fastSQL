using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.IndexExporters
{
    public interface IIndexExporter
    {
        string Id { get; }
        string Name { get; }
        IIndexExporter LoadOptions();
        IIndexExporter SetOptions(IEnumerable<OptionItem> options);
        IIndexExporter Save();
        IEnumerable<OptionItem> Options { get; }
        IEnumerable<OptionItem> GetOptionsTemplate();
        IIndexExporter SetIndex(string id, EntityType entityType);
        bool Export(out string message);
    }
}
