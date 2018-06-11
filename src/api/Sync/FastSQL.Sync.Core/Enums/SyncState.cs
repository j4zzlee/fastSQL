using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Enums
{
    [Flags]
    public enum SyncState
    {
        None = 0,
        HasData = 1,
        Invalid = 2
    }
}
