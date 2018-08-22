using FastSQL.App.Events;
using FastSQL.Core.UI.Interfaces;
using FastSQL.Sync.Core.Settings;
using Prism.Events;
using Syncfusion.Windows.Tools.Controls;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FastSQL.App.UserControls
{
    /// <summary>
    /// Interaction logic for UCSettingsContent.xaml
    /// </summary>
    public partial class UCSettingsContent : UserControl, IControlDefinition
    {
        public UCSettingContentViewModel ViewModel { get; set; }
        public IEnumerable<ISettingProvider> SettingProviders { get; set; }
        public IEventAggregator EventAggregator { get; set; }
        private string _currentSettingId = string.Empty;

        public UCSettingsContent()
        {
            InitializeComponent();
            Loaded += async (s, e) => await OnLoaded();
        }
        
        public void SetViewModel(UCSettingContentViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public void SetSettingProviders (IEnumerable<ISettingProvider> settingProviders)
        {
            SettingProviders = settingProviders;
        }

        public void SetEventAggregator(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

        public Task<int> OnLoaded()
        {
            DataContext = ViewModel;
            EventAggregator?.GetEvent<SelectSettingEvent>().Subscribe(OnSelectSetting);
            return Task.FromResult(0);
        }

        private void OnSelectSetting(SelectSettingEventArgument obj)
        {
            _currentSettingId = obj.SettingId;
            var currentSetting = SettingProviders.FirstOrDefault(s => s.Id == _currentSettingId);
            if (currentSetting == null)
            {
                return;
            }
            ViewModel.SetOptions(currentSetting.Options);
            ViewModel.SetCommands(currentSetting.Commands.Concat(new List<string> { "Save", "Validate" }));
            ViewModel.SetProvider(currentSetting);
        }

        public string Id { get => "NZWVJgnbIEOxA5UU8r8tNA=="; set { } }

        public string ControlName { get => "application_settings"; set { } }
        public string ControlHeader { get => "Application Settings"; set { } }

        public string Description { get => "Application Setting Details"; set { } }
        
        public string ActivatedById { get => "E9XCrgxIXUqlckiPX5VAQw"; set { } }

        public int DefaultState => (int)DockState.Document;
        
        public object Control => this;
    }
}
