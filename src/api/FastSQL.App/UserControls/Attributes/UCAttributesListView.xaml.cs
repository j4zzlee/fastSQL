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

namespace FastSQL.App.UserControls.Attributes
{
    /// <summary>
    /// Interaction logic for UCAttributesListView.xaml
    /// </summary>
    public partial class UCAttributesListView : UserControl, IControlDefinition
    {
        private readonly AttributesListViewViewModel viewModel;

        public UCAttributesListView(AttributesListViewViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            this.DataContext = viewModel;
            Loaded += (s, e) => viewModel.Loaded();
        }

        public string Id => "QzqMws4HH0GfDcDn/K8JRQ==";

        public string ControlName => "attribute_list_view_management";

        public string ControlHeader => "Attributes";

        public string Description => "List of attributes";
        
        public string ActivatedById => string.Empty;

        public int DefaultState => (int)DockState.Dock;

        public object Control => this;
    }
}
