using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Events
{
    public class SelectAttributeEvent: PubSubEvent<SelectAttributeEventArgument>
    {
    }

    public class SelectAttributeEventArgument
    {
        public string AttributeId { get; set; }
    }
}
