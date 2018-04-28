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

namespace FastSQL.App.UserControls.Entities
{
    /// <summary>
    /// Interaction logic for UCEntitiesListView.xaml
    /// </summary>
    public partial class UCEntitiesListView : UserControl, IControlDefinition
    {
        private readonly EntitiesListViewViewModel viewModel;

        public UCEntitiesListView(EntitiesListViewViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            this.DataContext = viewModel;
        }

        public string Id => "lS2j9IRSTE+c8TFL7LFgZA==";

        public string ControlName => "entity_list_view_management";

        public string ControlHeader => "Entities";

        public string Description => "List of Entities";
        
        public string ActivatedById => string.Empty;

        public int DefaultState => (int)DockState.Dock;

        public object Control => this;
    }
}
