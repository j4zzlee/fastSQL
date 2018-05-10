using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Events
{
    public class SelectIndexEvent: PubSubEvent<SelectIndexEventArgument>
    {
    }

    public class SelectIndexEventArgument
    {
        public IIndexModel IndexModel { get; set; }
    }
}
