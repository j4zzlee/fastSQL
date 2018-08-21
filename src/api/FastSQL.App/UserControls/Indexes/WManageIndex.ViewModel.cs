using FastSQL.App.Events;
using FastSQL.App.Interfaces;
using FastSQL.App.UserControls.DataGrid;
using FastSQL.App.UserControls.OutputView;
using FastSQL.Core;
using FastSQL.Core.Events;
using FastSQL.Core.Loggers;
using FastSQL.Core.UI.Models;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Constants;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Filters;
using FastSQL.Sync.Core.Indexer;
using FastSQL.Sync.Core.IndexExporters;
using FastSQL.Sync.Core.Mapper;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Puller;
using FastSQL.Sync.Core.Pusher;
using FastSQL.Sync.Core.Repositories;
using Newtonsoft.Json.Linq;
using Prism.Events;
using Serilog;
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
    public class WManageIndexViewModel : BaseViewModel, IDisposable
    {
        private IPusher _pusher;
        private IIndexer _indexer;
        private IPuller _puller;
        private IMapper _mapper;
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
        private readonly LoggerFactory loggerFactory;
        private readonly IEnumerable<IIndexExporter> indexExporters;
        private readonly IndexerManager indexerManager;
        private readonly PusherManager syncManager;
        private readonly MapperManager mapperManager;
        private readonly ResolverFactory resolverFactory;
        private ILogger logger;
        
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
                //dataGridViewModel.SetGridContextMenus(new List<string> {
                //    "Change",
                //    //"Change Range",
                //    "Change All",
                //    "Remove",
                //    //"Remove Range",
                //    "Remove All",
                //    "Sync",
                //    //"Sync Range",
                //    "Sync All"});
                dataGridViewModel.SetGridContextMenus(new List<MenuItemDefinition>
                {
                    new MenuItemDefinition
                    {
                        Name = "Change",
                        Children = new ObservableCollection<MenuItemDefinition>
                        {
                            new MenuItemDefinition
                            {
                                Name = "Selected",
                                CommandName = "ChangeSelected"
                            },
                            new MenuItemDefinition
                            {
                                Name = "All",
                                CommandName = "ChangeAll"
                            }
                        }
                    },
                    new MenuItemDefinition
                    {
                        Name = "Unchange",
                        Children = new ObservableCollection<MenuItemDefinition>
                        {
                            new MenuItemDefinition
                            {
                                Name = "Selected",
                                CommandName = "UnchangeSelected"
                            },
                            new MenuItemDefinition
                            {
                                Name = "All",
                                CommandName = "UnchangeAll"
                            }
                        }
                    },
                    new MenuItemDefinition
                    {
                        Name = "Remove",
                        Children = new ObservableCollection<MenuItemDefinition>
                        {
                            new MenuItemDefinition
                            {
                                Name = "Selected",
                                CommandName = "RemoveSelected"
                            },
                            new MenuItemDefinition
                            {
                                Name = "All",
                                CommandName = "RemoveAll"
                            }
                        }
                    },
                    new MenuItemDefinition
                    {
                        Name = "Process",
                        Children = new ObservableCollection<MenuItemDefinition>
                        {
                            new MenuItemDefinition
                            {
                                Name = "Selected",
                                CommandName = "ProcessSelected"
                            },
                            new MenuItemDefinition
                            {
                                Name = "All",
                                CommandName = "ProcessAll"
                            }
                        }
                    },
                    new MenuItemDefinition
                    {
                        Name = "Unprocess",
                        Children = new ObservableCollection<MenuItemDefinition>
                        {
                            new MenuItemDefinition
                            {
                                Name = "Selected",
                                CommandName = "UnprocessSelected"
                            },
                            new MenuItemDefinition
                            {
                                Name = "All",
                                CommandName = "UnprocessAll"
                            }
                        }
                    },
                    new MenuItemDefinition
                    {
                        Name = "Sync",
                        CommandName = "Sync"
                    }
                });
                dataGridViewModel.OnFilter += DataGridViewModel_OnFilter;
                dataGridViewModel.OnEvent += DataGridViewModel_OnEvent;
                OnPropertyChanged(nameof(DataGridViewModel));
            }
        }

        public void SetMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        private async void DataGridViewModel_OnEvent(object sender, Events.DataGridCommandEventArgument args)
        {
            var selectedItems = args.SelectedItems
                .Select(i => IndexItemModel.FromJObject(JObject.FromObject(i)));
            var selectedItemIds = selectedItems.Select(i => i["Id"].ToString());
            switch (args.CommandName)
            {
                case "ChangeSelected":
                    entityRepository.ChangeStateOfIndexedItems(_indexModel,
                        ItemState.Changed,
                        ItemState.Removed | ItemState.Invalid,
                        selectedItemIds.ToArray());
                    break;
                case "ChangeAll":
                    entityRepository.ChangeStateOfIndexedItems(_indexModel,
                        ItemState.Changed,
                        ItemState.Removed | ItemState.Invalid,
                        null);
                    break;
                case "UnchangeSelected":
                    entityRepository.ChangeStateOfIndexedItems(_indexModel,
                        ItemState.None,
                        ItemState.Changed | ItemState.Invalid,
                        selectedItemIds.ToArray());
                    break;
                case "UnchangeAll":
                    entityRepository.ChangeStateOfIndexedItems(_indexModel,
                        ItemState.None,
                        ItemState.Changed | ItemState.Invalid,
                        null);
                    break;
                case "RemoveSelected":
                    entityRepository.ChangeStateOfIndexedItems(_indexModel,
                        ItemState.Changed | ItemState.Removed,
                        ItemState.Invalid,
                        selectedItemIds.ToArray());
                    break;
                case "RemoveAll":
                    entityRepository.ChangeStateOfIndexedItems(_indexModel,
                        ItemState.Changed | ItemState.Removed,
                        ItemState.Invalid,
                        null);
                    break;
                case "ProcessSelected":
                    entityRepository.ChangeStateOfIndexedItems(_indexModel,
                        ItemState.Processed,
                        ItemState.Invalid,
                        selectedItemIds.ToArray());
                    break;
                case "ProcessAll":
                    entityRepository.ChangeStateOfIndexedItems(_indexModel,
                        ItemState.Processed,
                        ItemState.Invalid,
                        null);
                    break;
                case "UnprocessSelected":
                    entityRepository.ChangeStateOfIndexedItems(_indexModel,
                        ItemState.None,
                        ItemState.Processed | ItemState.Invalid,
                        selectedItemIds.ToArray());
                    break;
                case "UnprocessAll":
                    entityRepository.ChangeStateOfIndexedItems(_indexModel,
                        ItemState.None,
                        ItemState.Processed | ItemState.Invalid,
                        null);
                    break;
                case "Sync":
                    syncManager.SetIndex(_indexModel);
                    syncManager.SetIndexer(_indexer);
                    syncManager.SetPusher(_pusher);
                    await syncManager.Push(selectedItems.ToArray());
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
            IEnumerable<IIndexExporter> indexExporters,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            ConnectionRepository connectionRepository,
            LoggerFactory loggerFactory,
            IndexerManager indexerManager, 
            PusherManager syncManager,
            MapperManager mapperManager,
            ResolverFactory resolverFactory)
        {
            this.eventAggregator = eventAggregator;
            this.entityRepository = entityRepository;
            this.attributeRepository = attributeRepository;
            this.connectionRepository = connectionRepository;
            this.loggerFactory = loggerFactory;
            this.indexExporters = indexExporters;
            this.indexerManager = indexerManager;
            this.syncManager = syncManager;
            this.mapperManager = mapperManager;
            this.resolverFactory = resolverFactory;
        }
        
        private async void OnMapIndex(object obj)
        {
            try
            {
                IsLoading = true;
                await mapperManager
                    .SetIndex(_indexModel)
                    .SetMapper(_mapper)
                    .Map();
                await LoadData(null, DataGridContstants.PageLimit, 0, true);
                MessageBox.Show(
                    (Owner as Window) ?? Application.Current.MainWindow,
                    "Entities are mapped successfully",
                    "Sucess",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void OnInitIndex(object obj)
        {
            try
            {
                IsLoading = true;
                _puller.Init();
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
            this.logger = loggerFactory
                .WriteToApplication($"{_indexModel.EntityType} Index")
                .WriteToFile()
                .CreateApplicationLogger();
            this.indexerManager.OnReport(m =>
            {
                logger.Information(m);
            });
            _puller?.SetIndex(_indexModel);
            _indexer?.SetIndex(_indexModel);
            _pusher?.SetIndex(_indexModel);
            
            Initialized = _puller?.Initialized() == true && entityRepository?.Initialized(_indexModel) == true;

            if (Initialized)
            {
                await LoadData(null, DataGridContstants.PageLimit, 0, true);
            }
        }

        public void Dispose()
        {
            resolverFactory.Release(logger);
            //throw new NotImplementedException();
        }
    }
}
