using FastSQL.Core;
using System;
using System.Collections.Generic;

namespace FastSQL.Sync.Core
{
    public interface IPuller : IVendorVerifier
    {
        IPuller SetOptions(IEnumerable<OptionItem> options);
        IEnumerable<OptionItem> Options { get; }
        void PullAll();
        void PullRecent();
    }
}
