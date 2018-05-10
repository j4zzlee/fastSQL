using FastSQL.Sync.Core.Enums;
using System.Collections.Generic;

namespace FastSQL.Sync.Core
{
    public class PullResult
    {
        public SyncState Status { get; set; } = SyncState.Invalid;
        public object LastToken { get; set; }
        public IEnumerable<object> Data { get; set; }

        public bool IsValid()
        {
            return (Status & SyncState.Invalid) == 0;
        }

        public PullResult AddState(SyncState state)
        {
            Status = Status | state;
            return this;
        }
        public PullResult RemoveState(SyncState state)
        {
            Status = (Status | state) ^ state;
            return this;
        }
    }
}
