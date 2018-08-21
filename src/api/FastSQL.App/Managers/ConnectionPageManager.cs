using FastSQL.App.Interfaces;
using FastSQL.App.UserControls;
using FastSQL.App.UserControls.Connections;
using FastSQL.Core;
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
        private readonly ResolverFactory resolverFactory;
        private UCConnectionsListView ucConnectionListView;
        private UCConnectionsContent uCConnectionsContent;

        public string Id => "LI5b8oVn@#$%@asdf#@$MUqTxSHIgNj6wQ";

        public string Name => "Global Connections";

        public string Description => "Global Connections";

        public ConnectionPageManager(
            IEventAggregator eventAggregator,
            ResolverFactory resolverFactory)
        {
            this.eventAggregator = eventAggregator;
            this.resolverFactory = resolverFactory;
        }

        public IPageManager Apply()
        {
            if (ucConnectionListView == null)
            {
                ucConnectionListView = resolverFactory.Resolve<UCConnectionsListView>();
            }

            if (uCConnectionsContent == null)
            {
                uCConnectionsContent = resolverFactory.Resolve<UCConnectionsContent>();
            }

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
