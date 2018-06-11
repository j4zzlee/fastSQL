using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Enums
{
    public enum QueueItemState
    {
        None = 0,
        ByPassed = 1,
        Reported = 2,
        Success = 4,
        Failed = 8
    }
}
