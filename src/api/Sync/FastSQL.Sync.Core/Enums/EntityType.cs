using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Enums
{
    [Flags]
    public enum EntityType
    {
        Connection = 1,
        Entity = 2,
        Attribute = 4,
        Transformation = 8,
        Exporter = 16
    }
}
