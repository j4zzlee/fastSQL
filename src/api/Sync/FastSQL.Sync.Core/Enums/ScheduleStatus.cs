using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Enums
{
    [Flags]
    public enum ScheduleStatus
    {
        None = 0,
        Enabled = 1,
        RunsInParallel = 2
    }
}
