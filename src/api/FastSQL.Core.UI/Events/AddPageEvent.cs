using FastSQL.Core.UI.Interfaces;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.Core.UI.Events
{
    public class AddPageEvent: PubSubEvent<AddPageEventArgument>
    {
    }

    public class AddPageEventArgument
    {
        public IControlDefinition PageDefinition { get; set; }
    }
}
