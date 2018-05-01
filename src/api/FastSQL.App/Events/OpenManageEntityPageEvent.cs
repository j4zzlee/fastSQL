using FastSQL.Sync.Core.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Events
{
    public class OpenManageEntityPageEvent: PubSubEvent<OpenManageEntityPageEventArgument>
    {
    }

    public class OpenManageEntityPageEventArgument
    {
        public EntityModel Entity { get; set; }
    }
}
