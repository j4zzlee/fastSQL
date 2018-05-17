using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Indexer;
using FastSQL.Sync.Core.Mapper;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Puller;
using FastSQL.Sync.Core.Pusher;
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
        public IMapper Mapper { get; internal set; }
    }
}
