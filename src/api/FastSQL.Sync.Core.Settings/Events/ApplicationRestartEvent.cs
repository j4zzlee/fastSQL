using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Settings.Events
{
    public class ApplicationRestartEvent: PubSubEvent<ApplicationRestartEventArgument>
    {
    }

    public class ApplicationRestartEventArgument
    {
    }
}
