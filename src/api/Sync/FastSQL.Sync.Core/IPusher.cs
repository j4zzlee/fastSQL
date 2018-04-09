using FastSQL.Core;
using System;
using System.Collections.Generic;

namespace FastSQL.Sync.Core
{
    public interface IPusher: IVendorVerifier
    {
        IPusher SetOptions(IEnumerable<OptionItem> options);
        IEnumerable<OptionItem> Options { get; }
        void PushAll();
        void Push(Guid itemId);
    }
}
