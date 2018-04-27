using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Events
{
    public class SelectSettingEvent: PubSubEvent<SelectSettingEventArgument>
    {
    }

    public class SelectSettingEventArgument
    {
        public string SettingId { get; set; }
    }
}
