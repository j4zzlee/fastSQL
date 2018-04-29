using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Events
{
    public class SelectEntityEvent: PubSubEvent<SelectEntityEventArgument>
    {
    }

    public class SelectEntityEventArgument
    {
        public string EntityId { get; set; }
    }
}
