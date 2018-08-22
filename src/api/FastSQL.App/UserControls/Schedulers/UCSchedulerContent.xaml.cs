using FastSQL.Core;
using FastSQL.Core.UI.Interfaces;
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

namespace FastSQL.App.UserControls.Schedulers
{
    /// <summary>
    /// Interaction logic for UCSchedulerContent.xaml
    /// </summary>
    public partial class UCSchedulerContent : UserControl, IControlDefinition
    {
        private readonly UCSchedulerContentViewModel viewModel;
        private readonly ResolverFactory resolverFactory;

        public UCSchedulerContent(
            IEventAggregator eventAggregator,
            UCSchedulerContentViewModel viewModel,
            ResolverFactory resolverFactory)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            this.resolverFactory = resolverFactory;
            DataContext = this.viewModel;
            Loaded += async (s, e) => await viewModel.Loaded();
        }

        public string Id { get => "NZW8@!2_+38$@32VJgnbIEOxA5UU8r8tNA=="; set { } }

        public string ControlName { get => "schedulers_management"; set { } }
        public string ControlHeader { get => "Schedulers Management"; set { } }

        public string Description { get => "Schedulers Management"; set { } }

        public string ActivatedById { get; set; }

        public int DefaultState => (int)DockState.Document;

        public object Control => this;
    }
}
