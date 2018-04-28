using FastSQL.App.Interfaces;
using FastSQL.App.UserControls;
using FastSQL.Core.UI.Events;
using Prism.Events;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Managers
{
    public class SettingPageManager : IPageManager
    {
        private readonly IEventAggregator eventAggregator;
        private readonly UCSettingsListView uCSettingsListView;
        private readonly UCSettingsContent uCSettingsContent;

        public string Id => "LI5b8oVnMUqTxSHIgNj6wQ";

        public string Name => "Settings";

        public string Description => "FastSQL Settings Management";

        public SettingPageManager(
            IEventAggregator eventAggregator,
            UCSettingsListView uCSettingsListView,
            UCSettingsContent uCSettingsContent)
        {
            this.eventAggregator = eventAggregator;
            this.uCSettingsListView = uCSettingsListView;
            this.uCSettingsContent = uCSettingsContent;
        }

        public IPageManager Apply()
        {
            eventAggregator.GetEvent<AddPageEvent>().Publish(new AddPageEventArgument
            {
                PageDefinition = uCSettingsListView
            });

            eventAggregator.GetEvent<AddPageEvent>().Publish(new AddPageEventArgument
            {
                PageDefinition = uCSettingsContent
            });
            return this;
        }
    }
}
