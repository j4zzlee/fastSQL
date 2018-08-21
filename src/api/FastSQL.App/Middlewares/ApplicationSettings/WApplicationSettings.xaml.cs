using FastSQL.App.Events;
using FastSQL.Core;
using FastSQL.Sync.Core.Settings;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FastSQL.App.Middlewares.ApplicationSettings
{
    /// <summary>
    /// Interaction logic for WApplicationSettings.xaml
    /// </summary>
    public partial class WApplicationSettings : Window
    {
        private readonly IEventAggregator eventAggregator;
        private readonly ResolverFactory resolverFactory;

        public WApplicationSettings(
            WApplicationSettingsViewModel viewModel,
            IEventAggregator eventAggregator,
            ResolverFactory resolverFactory)
        {
            InitializeComponent();
            DataContext = viewModel;
            SettingContent.SetEventAggregator(eventAggregator);
            //
            SettingContent.SetViewModel(viewModel.SettingViewModel);
            SettingContent.OnLoaded();
            this.eventAggregator = eventAggregator;
            this.resolverFactory = resolverFactory;
            //,
            //IEnumerable<ISettingProvider> settingProviders
        }

        public void SetProviders(IEnumerable<ISettingProvider> providers)
        {
            SettingContent.SetSettingProviders(providers);
            SettingContent.OnLoaded();
        }

        public void SetProvider(ISettingProvider provider)
        {
            eventAggregator.GetEvent<SelectSettingEvent>().Publish(new SelectSettingEventArgument
            {
                SettingId = provider.Id
            });
        }
    }
}
