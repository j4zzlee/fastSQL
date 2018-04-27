using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Core.UI.Events
{
    public class ActivateControlEvent: PubSubEvent<ActivateControlEventArgument>
    {
    }

    public class ActivateControlEventArgument
    {
        public string ControlId { get; set; }
    }
}
