using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Enums
{
    [Flags]
    public enum ProcessorType
    {
        Entity = 1,
        Attribute = 2
    }
}
