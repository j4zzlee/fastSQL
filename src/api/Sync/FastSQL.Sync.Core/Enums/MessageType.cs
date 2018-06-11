using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Enums
{
    [Flags]
    public enum MessageType
    {
        None = 0,
        Information = 1,
        Error = 2,
        Report = 4,
        Exception = 8
    }
}
