using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Events
{
    public class ManageIndexLoadingEvent: PubSubEvent<ManageIndexLoadingEventArgument>
    {
    }

    public class ManageIndexLoadingEventArgument
    {
        public bool Loading { get; set; }
    }
}
