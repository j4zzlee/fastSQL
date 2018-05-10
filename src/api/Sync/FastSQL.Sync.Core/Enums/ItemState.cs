using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Enums
{
    [Flags]
    public enum ItemState
    {
        Removed = 4,
        Changed = 8,
        Processed = 16,
        Invalid = 32
    }
}
