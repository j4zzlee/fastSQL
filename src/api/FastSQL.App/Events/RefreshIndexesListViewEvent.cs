using FastSQL.Sync.Core.Enums;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Events
{
    public class RefreshIndexesListViewEvent: PubSubEvent<RefreshIndexesListViewEventArgument>
    {
    }

    public class RefreshIndexesListViewEventArgument
    {
        public string SelectedIndexId { get; set; }
        public EntityType SelectedIndexType { get; set; }
    }
}
