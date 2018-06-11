using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Enums
{
    [Flags]
    public enum ProcessorType
    {
        None = 0,
        Entity = 1,
        Attribute = 2
    }
}
