using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Events
{
    public class RefreshAttributeListEvent: PubSubEvent<RefreshAttributeListEventArgument>
    {
    }

    public class RefreshAttributeListEventArgument
    {
        public string SelectedAttributeId { get; set; }
    }
}
