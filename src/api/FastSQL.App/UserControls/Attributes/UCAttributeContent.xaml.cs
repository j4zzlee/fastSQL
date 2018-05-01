using FastSQL.App.Events;
using FastSQL.App.UserControls.Previews;
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

namespace FastSQL.App.UserControls.Attributes
{
    /// <summary>
    /// Interaction logic for UCAttributeContent.xaml
    /// </summary>
    public partial class UCAttributeContent : UserControl, IControlDefinition
    {
        private readonly AttributeContentViewModel viewModel;
        private readonly ResolverFactory resolverFactory;

        public UCAttributeContent(
            IEventAggregator eventAggregator,
            AttributeContentViewModel viewModel,
            ResolverFactory resolverFactory)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            this.resolverFactory = resolverFactory;
            this.DataContext = viewModel;
            eventAggregator.GetEvent<SelectAttributeEvent>().Subscribe(OnAttributeSelected);
            eventAggregator.GetEvent<OpenManageAttributePageEvent>().Subscribe(OnManageAttribute);
            eventAggregator.GetEvent<AttributePreviewPageEvent>().Subscribe(OnPreviewAttribute);
        }

        private void OnPreviewAttribute(AttributePreviewPageEventArgument obj)
        {
            var window = resolverFactory.Resolve<WPreviewData>();
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        private void OnManageAttribute(OpenManageAttributePageEventArgument obj)
        {
            var window = resolverFactory.Resolve<WManageAttribute>();
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        private void OnAttributeSelected(SelectAttributeEventArgument obj)
        {
            tbContainer.SelectedIndex = 0;
        }

        public string Id => "EFp0ZhUYMkSo3SY5Y6y7AA==";

        public string ControlName => "attribute_management";

        public string ControlHeader => "Attribute Management";

        public string Description => "Manage Attribute";
        
        public string ActivatedById => "QzqMws4HH0GfDcDn/K8JRQ==";

        public int DefaultState => (int) DockState.Document;

        public object Control => this;
    }
}
