using FastSQL.App.UserControls.Indexes;
using FastSQL.Core.UI.Interfaces;
using FastSQL.Sync.Core.Enums;
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
    /// Interaction logic for UCIndexesListView.xaml
    /// </summary>
    public partial class UCIndexesListView : UserControl, IControlDefinition
    {
        private readonly UCIndexesListViewViewModel viewModel;

        public UCIndexesListView(UCIndexesListViewViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            this.viewModel = viewModel;
            Loaded += (s, e) => viewModel.Loaded();
        }

        public string Id { get; set; } // => "lS2j9IRSTE+c8TFL7LFgZA==";

        public string ControlName { get; set; } // => "entity_list_view_management";

        public string ControlHeader { get; set; } // => "Entities";

        public string Description { get; set; } // => "List of Entities";

        public string ActivatedById { get; set; } // => string.Empty;

        public int DefaultState => (int)DockState.Dock;

        public void SetIndexType(EntityType indexType)
        {
            viewModel.SetIndexType(indexType);
        }

        public object Control => this;
    }
}
