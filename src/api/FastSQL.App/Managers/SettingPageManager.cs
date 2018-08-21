using FastSQL.App.Interfaces;
using FastSQL.App.UserControls;
using FastSQL.Core;
using FastSQL.Core.UI.Events;
using FastSQL.Core.UI.Interfaces;
using FastSQL.Sync.Core.Settings;
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
        private readonly IEnumerable<ISettingProvider> settingProviders;
        private readonly ResolverFactory resolverFactory;
        private UCSettingsListView uCSettingsListView;
        private UCSettingsContent uCSettingsContent;

        public string Id => "LI5b8oVnMUqTxSHIgNj6wQ";

        public string Name => "Settings";

        public string Description => "FastSQL Settings Management";

        public SettingPageManager(
            IEventAggregator eventAggregator,
            IEnumerable<ISettingProvider> settingProviders,
            ResolverFactory resolverFactory)
        {
            this.eventAggregator = eventAggregator;
            this.settingProviders = settingProviders;
            this.resolverFactory = resolverFactory;
        }

        public IPageManager Apply()
        {
            if (uCSettingsListView == null)
            {
                uCSettingsListView = resolverFactory.Resolve<UCSettingsListView>();
            }

            if (uCSettingsContent == null)
            {
                uCSettingsContent = resolverFactory.Resolve<UCSettingsContent>();
                uCSettingsContent.SetSettingProviders(settingProviders);
            }

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
