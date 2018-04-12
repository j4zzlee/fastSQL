using FastSQL.Sync.Core.Enums;
using System.Collections.Generic;

namespace FastSQL.Sync.Core
{
    public class PullResult
    {
        public SyncState Status { get; set; }
        public object LastToken { get; set; }
        public IEnumerable<object> Data { get; set; }
    }
}
