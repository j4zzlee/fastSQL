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
        private readonly UCSettingContentViewModel viewModel;
        private readonly IEnumerable<ISettingProvider> settingProviders;
        private string _currentSettingId = string.Empty;
        public UCSettingsContent(
            UCSettingContentViewModel viewModel,
            IEventAggregator eventAggregator,
            IEnumerable<ISettingProvider> settingProviders)
        {
            InitializeComponent();
            eventAggregator.GetEvent<SelectSettingEvent>().Subscribe(OnSelectSetting);
            this.viewModel = viewModel;
            this.settingProviders = settingProviders;
            this.DataContext = viewModel;
        }

        private void OnSelectSetting(SelectSettingEventArgument obj)
        {
            _currentSettingId = obj.SettingId;
            var currentSetting = settingProviders.FirstOrDefault(s => s.Id == _currentSettingId);
            viewModel.SetOptions(currentSetting.Options);
            viewModel.SetCommands(currentSetting.Commands.Concat(new List<string> { "Save", "Validate" }));
            viewModel.SetProvider(currentSetting);
        }

        public string Id => "NZWVJgnbIEOxA5UU8r8tNA==";

        public string ControlName => "Settings";

        public string Description => "Settings Details";

        public bool IsActive { get; set; }

        public string ActivatedById => "E9XCrgxIXUqlckiPX5VAQw";

        public int DefaultState => (int)DockState.Document;

        public int DefaultPosition => (int)DockAbility.Tabbed;

        public object Control => this;
    }
}
