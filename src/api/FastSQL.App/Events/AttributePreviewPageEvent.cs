using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Events
{
    public class AttributePreviewPageEvent: PubSubEvent<AttributePreviewPageEventArgument>
    {
    }

    public class AttributePreviewPageEventArgument
    {
        public AttributeModel Attribute { get; internal set; }
        public EntityModel Entity { get; internal set; }
        public IAttributePuller Puller { get; internal set; }
    }
}
