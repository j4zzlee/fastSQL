using FastSQL.Core;
using System;
using System.Collections.Generic;

namespace FastSQL.Sync.Core
{
    public interface IIndexer : IVendorVerifier
    {
        IIndexer SetOptions(IEnumerable<OptionItem> options);
        IEnumerable<OptionItem> Options { get; }
        void IndexNews();
        void IndexChanges();
        void IndexRemovals();
    }
}
