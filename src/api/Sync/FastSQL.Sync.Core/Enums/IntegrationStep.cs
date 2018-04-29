using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Enums
{
    [Flags]
    public enum IntegrationStep
    {
        Pull = 1,
        Index = 2,
        Queue = 3,
        Push = 4
    }
}
