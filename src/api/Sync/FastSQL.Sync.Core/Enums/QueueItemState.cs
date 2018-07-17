using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Enums
{
    public enum PushState
    {
        None = 0,
        ByPassed = 1,
        Reported = 2,
        Success = 4,
        Failed = 8,
        ValidationFailed = 16,
        RelatedItemNotFound = 32,
        RelatedItemNotSync = 64,
        UnexpectedError = 128
    }
}
