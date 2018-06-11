using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Enums
{
    [Flags]
    public enum ItemState
    {
        None = 0,
        Removed = 1,
        Changed = 2,
        Processed = 4,
        Invalid = 8,
        RelatedItemNotFound = 16,
        RelatedItemNotSynced = 32
    }
}
