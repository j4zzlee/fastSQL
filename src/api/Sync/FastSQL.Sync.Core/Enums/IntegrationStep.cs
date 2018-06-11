using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Enums
{
    [Flags]
    public enum IntegrationStep
    {
        None = 0,
        Pulling = 1,
        Pulled = 2,
        Indexing = 4,
        Indexed = 8,
        Queuing = 16,
        Queued = 32,
        Pushing = 64,
        Pushed = 128
    }
}
