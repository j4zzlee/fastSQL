using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Events
{
    public class RefreshReporterListEvent : PubSubEvent<RefreshReporterListEventArgument>
    {
    }

    public class RefreshReporterListEventArgument
    {
        public Guid SelectedReporterId { get; set; }
    }
}
