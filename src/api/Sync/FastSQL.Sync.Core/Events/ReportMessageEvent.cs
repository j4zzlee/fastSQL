using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Events
{
    public class ReportMessageEvent: PubSubEvent<ReportMessageEventArgument>
    {
    }

    public class ReportMessageEventArgument
    {
        public string Message { get; set; }
    }
}
