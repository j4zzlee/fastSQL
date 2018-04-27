using FastSQL.App.Events;
using FastSQL.Core;
using FastSQL.Core.UI.Interfaces;
using FastSQL.Sync.Core.Repositories;
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

namespace FastSQL.App.UserControls.Connections
{
    /// <summary>
    /// Interaction logic for UCConnectionsContent.xaml
    /// </summary>
    public partial class UCConnectionsContent : UserControl, IControlDefinition
    {
        private readonly UCConnectionsContentViewModel viewModel;
        private readonly ConnectionRepository connectionRepository;
        private readonly IEnumerable<IRichProvider> providers;
        private string _currentConnectionId;

        public UCConnectionsContent(
            UCConnectionsContentViewModel viewModel,
            ConnectionRepository connectionRepository,
            IEnumerable<IRichProvider> providers,
            IEventAggregator eventAggregator)
        {
            InitializeComponent();
            eventAggregator.GetEvent<SelectConnectionEvent>().Subscribe(OnSelectConnection);
            this.viewModel = viewModel;
            this.connectionRepository = connectionRepository;
            this.providers = providers;
            DataContext = viewModel;
        }

        private void OnSelectConnection(SelectConnectionEventArgument obj)
        {
            _currentConnectionId = obj.ConnectionId;
            var currentSetting = connectionRepository.GetById(_currentConnectionId);
            var options = connectionRepository.LoadOptions(currentSetting.Id);
            var provider = providers.FirstOrDefault(p => p.Id == currentSetting.ProviderId);
            provider.SetOptions(options.Select(o => new OptionItem { Name = o.Key, Value = o.Value }));
            viewModel.SetConnection(currentSetting);
            viewModel.SetOptions(provider.Options);
            viewModel.SetCommands(new List<string> { "Try Connect", "Save", "Delete" });
        }

        public string Id => "NZWVJgnbIEOxA5)#$)*%#*%UU8r8tNA==";

        public string ControlName => "Settings";

        public string Description => "Settings Details";

        public bool IsActive { get; set; }

        public string ActivatedById => "E9XCrgxIXUqlfewf2342@#@ckiPX5VAQw";

        public int DefaultState => (int)DockState.Document;

        public int DefaultPosition => (int)DockAbility.Tabbed;

        public object Control => this;
    }
}
