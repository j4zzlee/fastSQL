using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Events
{
    public class RefreshChannelListEvent: PubSubEvent<RefreshChannelListEventArgument>
    {
    }

    public class RefreshChannelListEventArgument
    {
        public Guid SelectedChannelId { get; set; }
    }
}
