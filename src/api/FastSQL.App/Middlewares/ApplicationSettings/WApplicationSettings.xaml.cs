using FastSQL.App.Events;
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

        public WApplicationSettings(
            WApplicationSettingsViewModel viewModel,
            IEventAggregator eventAggregator,
            IEnumerable<ISettingProvider> settingProviders)
        {
            InitializeComponent();
            DataContext = viewModel;
            SettingContent.SetEventAggregator(eventAggregator);
            SettingContent.SetSettingProviders(settingProviders);
            SettingContent.SetViewModel(viewModel.SettingViewModel);
            SettingContent.OnLoaded();
            this.eventAggregator = eventAggregator;
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
