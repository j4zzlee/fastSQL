using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Enums
{
    [Flags]
    public enum ItemState
    {
        Changed = 1,
        Removed = 2,
        Processed = 4,
        Invalid = 8
    }
}
