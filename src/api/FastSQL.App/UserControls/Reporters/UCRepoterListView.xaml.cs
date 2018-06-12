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

namespace FastSQL.App.UserControls.Reporters
{
    /// <summary>
    /// Interaction logic for UCRepoterListView.xaml
    /// </summary>
    public partial class UCRepoterListView : UserControl, IControlDefinition
    {
        public UCRepoterListView(UCRepoterListViewViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            Loaded += (s, e) => viewModel.Loaded();
        }

        public string Id
        {
            get => "E9lfewf2342@##($*(#*(#q#)@ckiPX5Qw";
            set { }
        }

        public string ControlName
        {
            get => "reporter_listview";
            set { }
        }
        public string ControlHeader { get => "Reporters"; set { } }
        public string Description { get => "Reporters"; set { } }

        public string ActivatedById { get => ""; set { } }

        public int DefaultState => (int)DockState.Dock;

        public object Control => this;
    }
}
