using FastSQL.App.Interfaces;
using FastSQL.App.UserControls;
using FastSQL.App.UserControls.MessageDeliveryChannels;
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
    public class MessageDeliveryChannelPageManager : IPageManager
    {
        private readonly IEventAggregator eventAggregator;
        private readonly ResolverFactory resolverFactory;
        private UCMessageDeliveryChannelListView ucListView;
        private UCMessageDeliveryChannelContent ucContent;

        public string Id => "LI5b8oVnMU@%#))qTxSHIgNj6wQ";

        public string Name => "MessageDeliveryChannel";

        public string Description => "Message Delivery Channel Management";

        public MessageDeliveryChannelPageManager(
            IEventAggregator eventAggregator,
            ResolverFactory resolverFactory)
        {
            this.eventAggregator = eventAggregator;
            this.resolverFactory = resolverFactory;
        }

        public IPageManager Apply()
        {
            if (ucListView == null)
            {
                ucListView = resolverFactory.Resolve<UCMessageDeliveryChannelListView>();
            }

            if (ucContent == null)
            {
                ucContent = resolverFactory.Resolve<UCMessageDeliveryChannelContent>();
            }

            eventAggregator.GetEvent<AddPageEvent>().Publish(new AddPageEventArgument
            {
                PageDefinition = ucListView
            });

            eventAggregator.GetEvent<AddPageEvent>().Publish(new AddPageEventArgument
            {
                PageDefinition = ucContent
            });
            return this;
        }
    }
}
