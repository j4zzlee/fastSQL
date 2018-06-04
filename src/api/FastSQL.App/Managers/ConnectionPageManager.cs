using FastSQL.App.Interfaces;
using FastSQL.App.UserControls;
using FastSQL.App.UserControls.Connections;
using FastSQL.Core.UI.Events;
using FastSQL.Core.UI.Interfaces;
using Prism.Events;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Managers
{
    public class ConnectionPageManager : IPageManager
    {
        private readonly IEventAggregator eventAggregator;
        private readonly UCConnectionsListView ucConnectionListView;
        private readonly UCConnectionsContent uCConnectionsContent;

        public string Id => "LI5b8oVn@#$%@asdf#@$MUqTxSHIgNj6wQ";

        public string Name => "Global Connections";

        public string Description => "Global Connections";

        public ConnectionPageManager(
            IEventAggregator eventAggregator,
            UCConnectionsListView ucConnectionListView,
            UCConnectionsContent uCConnectionsContent)
        {
            this.eventAggregator = eventAggregator;
            this.ucConnectionListView = ucConnectionListView;
            this.uCConnectionsContent = uCConnectionsContent;
        }

        public IPageManager Apply()
        {
            eventAggregator.GetEvent<AddPageEvent>().Publish(new AddPageEventArgument
            {
                PageDefinition = ucConnectionListView
            });
            
            eventAggregator.GetEvent<AddPageEvent>().Publish(new AddPageEventArgument
            {
                PageDefinition = uCConnectionsContent
            });
            return this;
        }
    }
}
