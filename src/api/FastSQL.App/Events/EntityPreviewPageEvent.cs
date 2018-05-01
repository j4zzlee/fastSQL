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
    public class EntityPreviewPageEvent: PubSubEvent<EntityPreviewPageEventArgument>
    {
    }

    public class EntityPreviewPageEventArgument
    {
        public IEntityPuller Puller { get; internal set; }
        public EntityModel Entity { get; internal set; }
    }
}
