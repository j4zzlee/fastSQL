using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Core.Events
{
    public class IntegrationOutputEvent: PubSubEvent<IntegrationOutputEventArgument>
    {
    }

    public class IntegrationOutputEventArgument
    {
        public string Channel { get; set; }
        public string Message { get; set; }
    }
}
