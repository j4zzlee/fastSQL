using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Events
{
    public class SelectConnectionEvent: PubSubEvent<SelectConnectionEventArgument>
    {
    }

    public class SelectConnectionEventArgument
    {
        public string ConnectionId { get; set; }
    }
}
