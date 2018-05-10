using FastSQL.Core.UI.Interfaces;
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
    /// Interaction logic for UCConnectionsListView.xaml
    /// </summary>
    public partial class UCConnectionsListView : UserControl, IControlDefinition
    {
        public UCConnectionsListView(UCConnectionListViewViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        public string Id { get => "E9XCrgxIXUqlfewf2342@#@ckiPX5VAQw"; set { } }

        public string Description { get => "Global Connections"; set { } }
        
        public string ActivatedById { get => string.Empty; set { } }

        public int DefaultState => (int)DockState.Dock;
        
        public object Control => this;

        public string ControlName { get => "connections_listview"; set { } }
        public string ControlHeader { get => "Connections"; set { } }
    }
}
