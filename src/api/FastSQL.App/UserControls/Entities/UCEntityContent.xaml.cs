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
    /// Interaction logic for UCEntityContent.xaml
    /// </summary>
    public partial class UCEntityContent : UserControl, IControlDefinition
    {
        private readonly EntityContentViewModel viewModel;

        public UCEntityContent(EntityContentViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            this.DataContext = viewModel;
        }

        public string Id => "1JFIy8jqlU2LKbwoDkCc7g==";

        public string ControlName => "entity_management";

        public string ControlHeader => "Entity Management";

        public string Description => "Manage Entity";
        
        public string ActivatedById => "lS2j9IRSTE+c8TFL7LFgZA==";

        public int DefaultState => (int)DockState.Document;

        public object Control => this;
    }
}
