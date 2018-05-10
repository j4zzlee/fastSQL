using FastSQL.App.Events;
using FastSQL.App.UserControls.Indexes;
using FastSQL.App.UserControls.Previews;
using FastSQL.Core;
using FastSQL.Core.UI.Interfaces;
using FastSQL.Sync.Core.Enums;
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

namespace FastSQL.App.UserControls
{
    /// <summary>
    /// Interaction logic for UCIndexDetail.xaml
    /// </summary>
    public partial class UCIndexDetail : UserControl, IControlDefinition
    {
        private readonly UCIndexDetailViewModel viewModel;
        private readonly ResolverFactory resolverFactory;
        private EntityType _indexType;

        public UCIndexDetail(
            IEventAggregator eventAggregator,
            UCIndexDetailViewModel viewModel,
            ResolverFactory resolverFactory)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            this.resolverFactory = resolverFactory;
            DataContext = this.viewModel;
            Loaded += (s, e) => viewModel.Loaded();

            eventAggregator.GetEvent<SelectIndexEvent>().Subscribe(OnSelectIndex);
            eventAggregator.GetEvent<OpenManageIndexPageEvent>().Subscribe(OnManageIndex);
            eventAggregator.GetEvent<OpenIndexPreviewPageEvent>().Subscribe(OnOpenPreviewPage);
        }

        private void OnSelectIndex(SelectIndexEventArgument obj)
        {
            if (obj.IndexModel.EntityType != _indexType)
            {
                return;
            }
            tbContainer.SelectedIndex = 0;
        }

        private void OnManageIndex(OpenManageIndexPageEventArgument obj)
        {
            if (obj.IndexModel.EntityType != _indexType)
            {
                return;
            }
            var window = resolverFactory.Resolve<WManageIndex>();
            window.Owner = Application.Current.MainWindow;
            window.Closed += (s, e) => resolverFactory.Release(window);
            window.SetPuller(obj.Puller);
            window.SetIndexer(obj.Indexer);
            window.SetPusher(obj.Pusher);
            window.SetIndex(obj.IndexModel);
            window.ShowDialog();
        }

        private void OnOpenPreviewPage(OpenIndexPreviewPageEventArgument obj)
        {
            if (obj.IndexModel.EntityType != _indexType)
            {
                return;
            }
            var window = resolverFactory.Resolve<WPreviewData>();
            window.Owner = Application.Current.MainWindow;
            window.Closed += (s, e) => resolverFactory.Release(window);
            window.SetPuller(obj.Puller);
            window.SetIndex(obj.IndexModel);
            window.ShowDialog();
        }

        /**
         * Below information will be set on PageManager (e.g: EntityPageManager, AttributePageManager)
         */
        public string Id { get; set; } // => "1JFIy8jqlU2LKbwoDkCc7g==";

        public string ControlName { get; set; } // => "entity_management";

        public string ControlHeader { get; set; } // => "Entity Management";

        public string Description { get; set; } // => "Manage Entity";

        public string ActivatedById { get; set; } // => "lS2j9IRSTE+c8TFL7LFgZA==";

        public int DefaultState => (int)DockState.Document;

        public object Control => this;

        public void SetIndexType(EntityType indexType)
        {
            _indexType = indexType;
            viewModel.SetIndexType(indexType);
        }
    }
}
