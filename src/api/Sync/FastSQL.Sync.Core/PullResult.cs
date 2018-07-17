using FastSQL.Sync.Core.Enums;
using System.Collections.Generic;

namespace FastSQL.Sync.Core
{
    public class PullResult
    {
        public PullState Status { get; set; } = PullState.Invalid;
        public object LastToken { get; set; }
        public IEnumerable<object> Data { get; set; }

        public bool IsValid()
        {
            return (Status & PullState.Invalid) == 0;
        }

        public PullResult AddState(PullState state)
        {
            Status = Status | state;
            return this;
        }
        public PullResult RemoveState(PullState state)
        {
            Status = (Status | state) ^ state;
            return this;
        }
    }
}
