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
    public class OpenIndexPreviewPageEvent: PubSubEvent<OpenIndexPreviewPageEventArgument>
    {
    }

    public class OpenIndexPreviewPageEventArgument
    {
        public IPuller Puller { get; set; }
        public IIndexModel IndexModel { get; set; }
    }
}
