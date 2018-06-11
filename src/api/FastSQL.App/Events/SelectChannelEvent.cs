using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Events
{
    public class SelectChannelEvent: PubSubEvent<SelectChannelEventArgument>
    {
    }

    public class SelectChannelEventArgument
    {
        public Guid ChannelId { get; set; }
    }
}
