using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Core.Events
{
    public class ApplicationOutputEvent: PubSubEvent<ApplicationOutputEventArgument>
    {
    }

    public class ApplicationOutputEventArgument
    {
        public string Channel { get; set; }
        public string Message { get; set; }
    }
}
