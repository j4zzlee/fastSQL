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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FastSQL.App.Events;
using FastSQL.App.UserControls.Indexes;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Models;
using Prism.Events;

namespace FastSQL.App.UserControls
{
    /// <summary>
    /// Interaction logic for WManageIndex.xaml
    /// </summary>
    public partial class WManageIndex : Window
    {
        private IIndexModel _indexModel;
        private IPusher _pusher;
        private IIndexer _indexer;
        private IPuller _puller;
        private readonly WManageIndexViewModel viewModel;

        public WManageIndex(WManageIndexViewModel viewModel, IEventAggregator eventAggregator)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            this.viewModel.SetOwner(this);
            this.DataContext = this.viewModel;
            this.Loaded += (s, e) => this.viewModel.Loaded();
           
            eventAggregator.GetEvent<ManageIndexLoadingEvent>().Subscribe(OnManageIndexLoading);
        }

        private void OnManageIndexLoading(ManageIndexLoadingEventArgument obj)
        {
            if (obj.Loading)
            {
                ((Storyboard)FindResource("WaitStoryboard")).Begin();
            }
        }

        private void btnForceChange_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            var mouseDownEvent =
                new MouseButtonEventArgs(Mouse.PrimaryDevice,
                    Environment.TickCount,
                    MouseButton.Right)
                {
                    RoutedEvent = Mouse.MouseUpEvent,
                    Source = this,
                };

            InputManager.Current.ProcessInput(mouseDownEvent);
        }

        public void SetIndex(IIndexModel indexModel)
        {
            _indexModel = indexModel;
            viewModel.SetIndex(indexModel);
        }

        public void SetPuller(IPuller puller)
        {
            _puller = puller;
            viewModel.SetPuller(puller);
        }

        public void SetIndexer(IIndexer indexer)
        {
            _indexer = indexer;
            viewModel.SetIndexer(indexer);
        }

        public void SetPusher(IPusher pusher)
        {
            _pusher = pusher;
            viewModel.SetPusher(pusher);
        }
    }
}
