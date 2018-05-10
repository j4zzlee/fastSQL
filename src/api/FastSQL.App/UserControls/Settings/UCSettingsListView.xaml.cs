using FastSQL.Core.UI.Interfaces;
using FastSQL.Sync.Core.Settings;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for UCSettingsListView.xaml
    /// </summary>
    public partial class UCSettingsListView : UserControl, IControlDefinition
    {
        public string Id { get => "E9XCrgxIXUqlckiPX5VAQw"; set { } }

        public string Description { get => "Settings"; set { } }
        
        public string ActivatedById { get => string.Empty; set { } }

        public int DefaultState => (int)DockState.Dock;
        
        public object Control => this;

        public string ControlName { get => "application_settings_listview"; set { } }
        public string ControlHeader { get => "Application Settings"; set { } }


        public UCSettingsListView(UCSettingsListViewViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
