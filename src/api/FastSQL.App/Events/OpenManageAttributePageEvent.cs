using FastSQL.Sync.Core.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Events
{
    public class OpenManageAttributePageEvent: PubSubEvent<OpenManageAttributePageEventArgument>
    {
    }

    public class OpenManageAttributePageEventArgument
    {
        public EntityModel Entity { get; set; }
        public AttributeModel Attribute { get; set; }
    }
}
