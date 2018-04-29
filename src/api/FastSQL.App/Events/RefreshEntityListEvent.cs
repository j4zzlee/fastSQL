using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Events
{
    public class RefreshEntityListEvent: PubSubEvent<RefreshEntityListEventArgument>
    {
    }

    public class RefreshEntityListEventArgument
    {
        public string SelectedEntityId { get; set; }
    }
}
