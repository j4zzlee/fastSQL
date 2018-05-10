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
    public class OpenManageIndexPageEvent: PubSubEvent<OpenManageIndexPageEventArgument>
    {
    }

    public class OpenManageIndexPageEventArgument
    {
        public IIndexModel IndexModel { get; set; }
        public IPuller Puller { get; set; }
        public IIndexer Indexer { get; set; }
        public IPusher Pusher { get; set; }
    }
}
