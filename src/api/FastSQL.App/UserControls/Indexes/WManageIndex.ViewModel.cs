using FastSQL.App.Events;
using FastSQL.App.Interfaces;
using FastSQL.App.UserControls.DataGrid;
using FastSQL.App.UserControls.OutputView;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Constants;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Filters;
using FastSQL.Sync.Core.Indexer;
using FastSQL.Sync.Core.IndexExporters;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows;

namespace FastSQL.App.UserControls.Indexes
{
    public class WManageIndexViewModel : BaseViewModel
    {
        private IPusher _pusher;
        private IIndexer _indexer;
        private IPuller _puller;
        private IIndexModel _indexModel;

        private bool _initialized;
        private DataGridViewModel dataGridViewModel;
        private string _modelName;
        private bool _isLoading;
        private UCOutputViewViewModel _outputViewViewModel;
        private readonly IEventAggregator eventAggregator;
        private readonly EntityRepository entityRepository;
        private readonly AttributeRepository attributeRepository;
        private readonly ConnectionRepository connectionRepository;
        private readonly IEnumerable<IIndexExporter> indexExporters;
        private readonly IndexerManager indexerManager;

        public BaseCommand InitIndexCommand => new BaseCommand(o => true, OnInitIndex);
        public BaseCommand UpdateIndexCommand => new BaseCommand(o => true, OnUpdateIndex);
        public BaseCommand MapIndexCommand => new BaseCommand(o => true, OnMapIndex); // Only entity has map index
        
        public UCOutputViewViewModel OutputViewViewModel {
            get => _outputViewViewModel;
            set
            {
                _outputViewViewModel = value;
                OnPropertyChanged(nameof(OutputViewViewModel));
            }
        }
        public DataGridViewModel DataGridViewModel
        {
            get => dataGridViewModel;
            set
            {
                dataGridViewModel = value;
                dataGridViewModel.SetGridContextMenus(new List<string> {
                    "Change",
                    //"Change Range",
                    "Change All",
                    "Remove",
                    //"Remove Range",
                    "Remove All",
                    "Sync",
                    //"Sync Range",
                    "Sync All"});
                dataGridViewModel.OnFilter += DataGridViewModel_OnFilter;
                dataGridViewModel.OnEvent += DataGridViewModel_OnEvent;
                OnPropertyChanged(nameof(DataGridViewModel));
            }
        }

        private async void DataGridViewModel_OnEvent(object sender, Events.DataGridCommandEventArgument args)
        {
            var selectedItems = args.SelectedItems
                .Select(i => {
                    var obj = IndexItemModel.FromObject(i);
                    return obj["Id"].ToString();
                });
            switch (args.CommandName)
            {
                case "Change":
                    entityRepository.ChangeIndexedItems(_indexModel,
                        ItemState.Changed,
                        ItemState.Removed | ItemState.Invalid | ItemState.Processed,
                        selectedItems.ToArray());
                    break;
                case "Change All":
                    entityRepository.ChangeIndexedItems(_indexModel,
                        ItemState.Changed,
                        ItemState.Removed | ItemState.Invalid | ItemState.Processed,
                        null);
                    break;
                case "Remove":
                    entityRepository.ChangeIndexedItems(_indexModel,
                        ItemState.Changed | ItemState.Removed,
                        ItemState.Invalid | ItemState.Processed,
                        selectedItems.ToArray());
                    break;
                case "Remove All":
                    entityRepository.ChangeIndexedItems(_indexModel,
                        ItemState.Changed | ItemState.Removed,
                        ItemState.Invalid | ItemState.Processed,
                        null);
                    break;
                case "Sync":
                    break;
            }
            await LoadData(null, DataGridViewModel.GetLimit(), DataGridViewModel.GetOffset(), true);
        }

        private async void DataGridViewModel_OnFilter(object sender, FilterArguments args)
        {
            await LoadData(args.Filters, args.Limit, args.Offset, false);
        }

        private async Task LoadData(IEnumerable<FilterArgument> filters, int limit, int offset, bool reset)
        {
            IsLoading = true;
            try
            {
                await Task.Run(() =>
                {
                    var items = entityRepository.GetIndexedItems(_indexModel, filters, limit, offset, out int totalCount);
                    if (reset)
                    {
                        DataGridViewModel.CleanFilters();
                    }
                    DataGridViewModel.SetTotalCount(totalCount);
                    DataGridViewModel.SetOffset(limit, offset);
                    DataGridViewModel.SetData(items);
                });
            }
            finally
            {
                IsLoading = false;
            }
        }

        public ObservableCollection<IIndexExporter> Exporters
        {
            get => new ObservableCollection<IIndexExporter>(indexExporters);
        }

        public bool Initialized
        {
            get => _initialized;
            set
            {
                _initialized = value;
                OnPropertyChanged(nameof(Initialized));
            }
        }
        
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
                eventAggregator.GetEvent<ManageIndexLoadingEvent>().Publish(new ManageIndexLoadingEventArgument
                {
                    Loading = value
                });
            }
        }

        public string ModelName
        {
            get => _modelName;
            set
            {
                _modelName = value;
                OnPropertyChanged(nameof(ModelName));
            }
        }

        public WManageIndexViewModel(
            IEventAggregator eventAggregator,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            ConnectionRepository connectionRepository,
            IEnumerable<IIndexExporter> indexExporters,
            IndexerManager indexerManager)
        {
            this.eventAggregator = eventAggregator;
            this.entityRepository = entityRepository;
            this.attributeRepository = attributeRepository;
            this.connectionRepository = connectionRepository;
            this.indexExporters = indexExporters;
            this.indexerManager = indexerManager;
        }
        
        private void OnMapIndex(object obj)
        {
            throw new NotImplementedException();
        }

        private async void OnInitIndex(object obj)
        {
            try
            {
                IsLoading = true;
                var pullerInit = _puller.Init(out string pullerInitMessage);
                if (!pullerInit)
                {
                    MessageBox.Show(
                        (Owner as Window) ?? Application.Current.MainWindow,
                        pullerInitMessage,
                        "Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                entityRepository.Init(_indexModel);
                indexerManager.SetIndex(_indexModel);
                indexerManager.SetIndexer(_indexer);
                indexerManager.SetPuller(_puller);
                await indexerManager.PullAll(true);
                await LoadData(null, DataGridContstants.PageLimit, 0, true);
                MessageBox.Show(
                    (Owner as Window) ?? Application.Current.MainWindow,
                    "Initialized",
                    "Sucess",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void OnUpdateIndex(object obj)
        {
            try
            {
                IsLoading = true;
                indexerManager.SetIndex(_indexModel);
                indexerManager.SetIndexer(_indexer);
                indexerManager.SetPuller(_puller);
                await indexerManager.PullAll(false);
                await LoadData(null, DataGridContstants.PageLimit, 0, true);
                MessageBox.Show(
                    (Owner as Window) ?? Application.Current.MainWindow,
                    "Done.",
                    "Sucess",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void SetPusher(IPusher pusher)
        {
            _pusher = pusher;
        }

        public void SetIndexer(IIndexer indexer)
        {
            _indexer = indexer;
        }

        public void SetPuller(IPuller puller)
        {
            _puller = puller;
        }

        public void SetIndex(IIndexModel indexModel)
        {
            _indexModel = indexModel;
            ModelName = _indexModel?.Name ?? "Manage Index";
        }

        public async void Loaded()
        {
            if (_indexModel.EntityType == EntityType.Entity)
            {
                (_puller as IEntityPuller)?.SetEntity(_indexModel as EntityModel);
                (_pusher as IEntityPusher)?.SetEntity(_indexModel as EntityModel);
                (_indexer as IEntityIndexer)?.SetEntity(_indexModel as EntityModel);
            }
            else
            {
                var attr = _indexModel as AttributeModel;
                var entity = entityRepository.GetById(attr.Id.ToString());
                (_puller as IAttributePuller)?.SetAttribute(attr, entity);
                (_pusher as IAttributePusher)?.SetAttribute(attr, entity);
                (_indexer as IAttributeIndexer)?.SetAttribute(attr, entity);
            }

            Initialized = _puller.Initialized() && entityRepository.Initialized(_indexModel);

            if (Initialized)
            {
                await LoadData(null, DataGridContstants.PageLimit, 0, true);
            }
        }
    }
}
