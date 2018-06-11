using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Events
{
    public class SelectReporterEvent: PubSubEvent<SelectReporterEventArgument>
    {
    }

    public class SelectReporterEventArgument
    {
        public Guid ReporterId { get; set; }
    }
}
