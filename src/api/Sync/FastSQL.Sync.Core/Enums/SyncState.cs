using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Enums
{
    [Flags]
    public enum SyncState
    {
        None = 1,
        HasData = 2,
        Invalid = 4
    }
}
