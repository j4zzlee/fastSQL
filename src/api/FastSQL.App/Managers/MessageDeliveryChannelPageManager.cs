using FastSQL.App.Interfaces;
using FastSQL.App.UserControls;
using FastSQL.App.UserControls.MessageDeliveryChannels;
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
        private readonly UCMessageDeliveryChannelListView ucListView;
        private readonly UCMessageDeliveryChannelContent ucContent;

        public string Id => "LI5b8oVnMU@%#))qTxSHIgNj6wQ";

        public string Name => "MessageDeliveryChannel";

        public string Description => "Message Delivery Channel Management";

        public MessageDeliveryChannelPageManager(
            IEventAggregator eventAggregator,
            UCMessageDeliveryChannelListView uCSettingsListView,
            UCMessageDeliveryChannelContent uCSettingsContent)
        {
            this.eventAggregator = eventAggregator;
            this.ucListView = uCSettingsListView;
            this.ucContent = uCSettingsContent;
        }

        public IPageManager Apply()
        {
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
