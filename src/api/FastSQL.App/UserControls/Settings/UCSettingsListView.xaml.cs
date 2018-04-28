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
        public string Id => "E9XCrgxIXUqlckiPX5VAQw";

        public string Description => "Settings";

        public bool IsActive { get; set; }

        public string ActivatedById => string.Empty;

        public int DefaultState => (int)DockState.Dock;

        public int DefaultPosition => (int)DockAbility.Left;

        public object Control => this;

        public string ControlName => "application_settings";
        public string ControlHeader => "Applicatioin Settings";


        public UCSettingsListView(UCSettingsListViewViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
