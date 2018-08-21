using FastSQL.App.Events;
using FastSQL.App.Interfaces;
using FastSQL.App.UserControls.Transformers;
using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Indexer;
using FastSQL.Sync.Core.Mapper;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Puller;
using FastSQL.Sync.Core.Pusher;
using FastSQL.Sync.Core.Repositories;
using Prism.Events;
using DateTimeExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace FastSQL.App.UserControls.Indexes
{
    public class UCIndexDetailViewModel : BaseViewModel
    {
        private EntityType _indexType;
        private IIndexModel _indexModel;
        private readonly IEventAggregator eventAggregator;
        private readonly IEnumerable<IProcessor> processors;
        private readonly IEnumerable<IPuller> pullers;
        private readonly IEnumerable<IIndexer> indexers;
        private readonly IEnumerable<IPusher> pushers;
        private readonly IEnumerable<IMapper> mappers;
        private readonly IEnumerable<ITransformer> transformers;
        
        private readonly EntityRepository entityRepository;
        private readonly AttributeRepository attributeRepository;
        private readonly ConnectionRepository connectionRepository;
        private ConnectionModel _sourceConnection;
        private ConnectionModel _destinationConnection;
        private IProcessor _sourceProcessor;
        private IProcessor _destinationProcessor;

        private ObservableCollection<string> _commands;
        private ObservableCollection<OptionItemViewModel> _pullerOptions;
        private ObservableCollection<OptionItemViewModel> _indexerOptions;
        private ObservableCollection<OptionItemViewModel> _pusherOptions;
        private ObservableCollection<OptionItemViewModel> _mapperOptions;
        private ObservableCollection<ConnectionModel> _sourceConnections;
        private ObservableCollection<IProcessor> _sourceProcessors;
        private ObservableCollection<ConnectionModel> _destinationConnections;
        private ObservableCollection<IProcessor> _destinationProcessors;

        private UCIndexDependenciesViewModel _entityDependencyViewModel;
        private UCIndexDependenciesViewModel _attributeDependencyViewModel;
        private UCTransformationConfigureViewModel _transformationConfigureViewModel;
        private string _name;
        private string _description;
        private bool _enabled;
        private EntityModel _selectedEntity;
        private ObservableCollection<EntityModel> _entities;
        private IPuller _puller;
        private IIndexer _indexer;
        private IPusher _pusher;
        private IMapper _mapper;

        public BaseCommand ApplyCommand => new BaseCommand(o => true, OnApplyCommand);

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public bool IsAttribute
        {
            get => _indexType == EntityType.Attribute;
            set
            {
                OnPropertyChanged(nameof(IsAttribute));
            }
        }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                OnPropertyChanged(nameof(Enabled));
            }
        }

        public string SourceViewName
        {
            get => _indexModel?.SourceViewName ?? "N/A";
            set
            {
                if (_indexModel != null)
                {
                    _indexModel.SourceViewName = value;
                    OnPropertyChanged(nameof(SourceViewName));
                }
            }
        }

        public string ValueTableName
        {
            get => _indexModel?.ValueTableName ?? "N/A";
            set
            {
                if (_indexModel != null)
                {
                    _indexModel.ValueTableName = value;
                    OnPropertyChanged(nameof(ValueTableName));
                }
            }
        }

        public string NewValueTableName
        {
            get => _indexModel?.NewValueTableName ?? "N/A";
            set
            {
                if (_indexModel != null)
                {
                    _indexModel.NewValueTableName = value;
                    OnPropertyChanged(nameof(NewValueTableName));
                }
            }
        }

        public string OldValueTableName
        {
            get => _indexModel?.OldValueTableName ?? "N/A";
            set
            {
                if (_indexModel != null)
                {
                    _indexModel.OldValueTableName = value;
                    OnPropertyChanged(nameof(OldValueTableName));
                }
            }
        }

        public EntityModel SelectedEntity
        {
            get => _selectedEntity;
            set
            {
                _selectedEntity = value;
                OnPropertyChanged(nameof(SelectedEntity));
            }
        }

        public ConnectionModel SelectedSourceConnection
        {
            get => _sourceConnection;
            set
            {
                _sourceConnection = value;
                OnPropertyChanged(nameof(SelectedSourceConnection));
            }
        }

        public ConnectionModel SelectedDestinationConnection
        {
            get => _destinationConnection;
            set
            {
                _destinationConnection = value;
                OnPropertyChanged(nameof(SelectedDestinationConnection));
            }
        }

        public IProcessor SelectedSourceProcessor
        {
            get => _sourceProcessor;
            set
            {
                _sourceProcessor = value;
                OnPropertyChanged(nameof(SelectedSourceProcessor));
            }
        }

        public IProcessor SelectedDestinationProcessor
        {
            get => _destinationProcessor;
            set
            {
                _destinationProcessor = value;
                OnPropertyChanged(nameof(SelectedDestinationProcessor));
            }
        }

        public ObservableCollection<EntityModel> Entities
        {
            get => _entities;
            set
            {
                _entities = value;
                OnPropertyChanged(nameof(Entities));
            }
        }

        public ObservableCollection<OptionItemViewModel> PullerOptions
        {
            get
            {
                return _pullerOptions;
            }
            set
            {
                _pullerOptions = value;
                OnPropertyChanged(nameof(PullerOptions));
            }
        }

        public ObservableCollection<OptionItemViewModel> IndexerOptions
        {
            get
            {
                return _indexerOptions;
            }
            set
            {
                _indexerOptions = value;
                OnPropertyChanged(nameof(IndexerOptions));
            }
        }

        public ObservableCollection<OptionItemViewModel> PusherOptions
        {
            get
            {
                return _pusherOptions;
            }
            set
            {
                _pusherOptions = value;
                OnPropertyChanged(nameof(PusherOptions));
            }
        }

        public ObservableCollection<OptionItemViewModel> MapperOptions
        {
            get
            {
                return _mapperOptions;
            }
            set
            {
                _mapperOptions = value;
                OnPropertyChanged(nameof(MapperOptions));
            }
        }

        public ObservableCollection<ConnectionModel> SourceConnections
        {
            get
            {
                return _sourceConnections;
            }
            set
            {
                _sourceConnections = value;
                OnPropertyChanged(nameof(SourceConnections));
            }
        }

        public ObservableCollection<IProcessor> SourceProcessors
        {
            get
            {
                return _sourceProcessors;
            }
            set
            {
                _sourceProcessors = value;
                OnPropertyChanged(nameof(SourceProcessors));
            }
        }

        public ObservableCollection<ConnectionModel> DestinationConnections
        {
            get
            {
                return _destinationConnections;
            }
            set
            {
                _destinationConnections = value;
                OnPropertyChanged(nameof(DestinationConnections));
            }
        }

        public ObservableCollection<IProcessor> DestinationProcessors
        {
            get
            {
                return _destinationProcessors;
            }
            set
            {
                _destinationProcessors = value;
                OnPropertyChanged(nameof(DestinationProcessors));
            }
        }

        public ObservableCollection<string> Commands
        {
            get => _commands;
            set
            {
                _commands = value ?? new ObservableCollection<string>();
                OnPropertyChanged(nameof(Commands));
            }
        }

        public UCIndexDependenciesViewModel EntityDependencyViewModel
        {
            get => _entityDependencyViewModel;
            set
            {
                _entityDependencyViewModel = value;
                OnPropertyChanged(nameof(EntityDependencyViewModel));
            }
        }

        public UCIndexDependenciesViewModel AttributeDependencyViewModel
        {
            get => _attributeDependencyViewModel;
            set
            {
                _attributeDependencyViewModel = value;
                OnPropertyChanged(nameof(AttributeDependencyViewModel));
            }
        }

        public UCTransformationConfigureViewModel TransformationConfigureViewModel
        {
            get => _transformationConfigureViewModel;
            set
            {
                _transformationConfigureViewModel = value;
                OnPropertyChanged(nameof(TransformationConfigureViewModel));
            }
        }
        
        public UCIndexDetailViewModel(
            IEventAggregator eventAggregator,
            IEnumerable<IProcessor> processors,
            IEnumerable<IPuller> pullers,
            IEnumerable<IIndexer> indexers,
            IEnumerable<IPusher> pushers,
            IEnumerable<IMapper> mappers,
            IEnumerable<ITransformer> transformers,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            ConnectionRepository connectionRepository)
        {
            this.eventAggregator = eventAggregator;
            this.processors = processors;
            this.entityRepository = entityRepository;
            this.attributeRepository = attributeRepository;
            this.connectionRepository = connectionRepository;

            this.pullers = pullers;
            this.indexers = indexers;
            this.pushers = pushers;
            this.mappers = mappers;
            this.transformers = transformers;

            Entities = new ObservableCollection<EntityModel>(entityRepository.GetAll());

            // Need to duplication code here, weird behavior of WPF
            SourceConnections = new ObservableCollection<ConnectionModel>(connectionRepository.GetAll());
            DestinationConnections = new ObservableCollection<ConnectionModel>(connectionRepository.GetAll());

            Commands = new ObservableCollection<string>(new List<string> { "Save", "New", "Delete", "Preview" }); // , "Manage"

            eventAggregator.GetEvent<SelectIndexEvent>().Subscribe(OnSelectIndex);
            eventAggregator.GetEvent<RefreshConnectionListEvent>().Subscribe(OnConnectionsChanged);
            eventAggregator.GetEvent<RefreshIndexesListViewEvent>().Subscribe(OnIndexesChanged);
        }
        
        public void FilterProcessors()
        {
            if (_indexType == EntityType.Entity)
            {
                var allProcessors = processors.Where(p => p.Type == ProcessorType.Entity);
                if (SelectedSourceConnection != null)
                {
                    SourceProcessors = new ObservableCollection<IProcessor>(allProcessors
                        .Where(ps => pullers.Where(p => typeof(IEntityPuller).IsAssignableFrom(p.GetType()))
                            .Select(p => p as IEntityPuller)
                            .Any(p => p.IsImplemented(ps.Id, SelectedSourceConnection.ProviderId))));
                } 
                else
                {
                    SourceProcessors = new ObservableCollection<IProcessor>(allProcessors);
                }

                if (SelectedDestinationConnection != null)
                {
                    DestinationProcessors = new ObservableCollection<IProcessor>(allProcessors
                        .Where(ps => pushers.Where(p => typeof(IEntityPusher).IsAssignableFrom(p.GetType()))
                            .Select(p => p as IEntityPusher)
                            .Any(p => p.IsImplemented(ps.Id, SelectedDestinationConnection.ProviderId))));
                }
                else
                {
                    DestinationProcessors = new ObservableCollection<IProcessor>(allProcessors);
                }
            }
            else
            {
                var allProcessors = processors.Where(p => p.Type == ProcessorType.Attribute);
                if (SelectedSourceConnection != null && SelectedEntity != null)
                {
                    SourceProcessors = new ObservableCollection<IProcessor>(allProcessors
                        .Where(ps => pullers.Where(p => typeof(IAttributePuller).IsAssignableFrom(p.GetType()))
                            .Select(p => p as IAttributePuller)
                            .Any(p => p.IsImplemented(ps.Id, SelectedEntity.SourceProcessorId, SelectedSourceConnection.ProviderId))));
                }
                else
                {
                    SourceProcessors = new ObservableCollection<IProcessor>(allProcessors);
                }

                if (SelectedDestinationConnection != null && SelectedEntity != null)
                {
                    DestinationProcessors = new ObservableCollection<IProcessor>(allProcessors
                        .Where(ps => pushers.Where(p => typeof(IAttributePusher).IsAssignableFrom(p.GetType()))
                            .Select(p => p as IAttributePusher)
                            .Any(p => p.IsImplemented(ps.Id, SelectedEntity.DestinationProcessorId, SelectedDestinationConnection.ProviderId))));
                }
                else
                {
                    DestinationProcessors = new ObservableCollection<IProcessor>(allProcessors);
                }
            }
        }

        private void OnSelectIndex(SelectIndexEventArgument args)
        {
            if (args.IndexModel.EntityType != _indexType)
            {
                return;
            }

            IsAttribute = _indexType == EntityType.Attribute;
            _indexModel = args.IndexModel;
            if (_indexType == EntityType.Attribute)
            {
                var attrModel = _indexModel as AttributeModel;
                SelectedEntity = Entities.FirstOrDefault(e => e.Id == attrModel.EntityId);
            }

            Name = _indexModel.Name;
            Description = _indexModel.Description;
            Enabled = !_indexModel.HasState(EntityState.Disabled);
            SourceViewName = _indexModel.SourceViewName;
            ValueTableName = _indexModel.ValueTableName;
            OldValueTableName = _indexModel.OldValueTableName;
            NewValueTableName = _indexModel.NewValueTableName;

            SelectedSourceConnection = SourceConnections.FirstOrDefault(c => c.Id == _indexModel.SourceConnectionId);
            SelectedDestinationConnection = DestinationConnections.FirstOrDefault(c => c.Id == _indexModel.DestinationConnectionId);
            FilterProcessors();
            //LoadOptions();
            SelectedSourceProcessor = SourceProcessors.FirstOrDefault(p => p.Id == _indexModel.SourceProcessorId);
            SelectedDestinationProcessor = DestinationProcessors.FirstOrDefault(p => p.Id == _indexModel.DestinationProcessorId);

            EntityDependencyViewModel.SetTargetType(_indexType);
            AttributeDependencyViewModel.SetTargetType(_indexType);

            EntityDependencyViewModel.SetDependencyType(EntityType.Entity);
            AttributeDependencyViewModel.SetDependencyType(EntityType.Attribute);

            EntityDependencyViewModel.SetIndex(_indexModel);
            AttributeDependencyViewModel.SetIndex(_indexModel);
            TransformationConfigureViewModel.SetIndex(_indexModel);

            Commands = new ObservableCollection<string>(new List<string> { "Save", "New", "Delete", "Preview", "Manage" });
        }

        private void OnApplyCommand(object obj)
        {
            var commandText = obj.ToString();
            if (commandText == "Manage")
            {
                Manage();
                return;
            }

            if (commandText == "Preview")
            {
                Preview();
                return;
            }
            var message = "Command not available";
            var success = false;
            switch (commandText)
            {
                case "Save":
                    success = Save(out message);
                    break;
                case "New":
                    success = New(out message);
                    break;
                case "Delete":
                    success = Delete(out message);
                    break;
            }
            MessageBox.Show(
                Owner as Window ?? Application.Current.MainWindow,
                message,
                success ? "Success" : "Failed",
                MessageBoxButton.OK,
                success ? MessageBoxImage.Information : MessageBoxImage.Error);

        }

        private void Preview()
        {
            if (SelectedSourceConnection == null || SelectedSourceProcessor == null)
            {
                MessageBox.Show(
                  Owner as Window ?? Application.Current.MainWindow,
                  $"Missing basic configuration for source",
                  "Failed",
                  MessageBoxButton.OK,
                  MessageBoxImage.Error);
                return;
            }
            
            _puller.SetOptions(PullerOptions.Select(o => new OptionItem { Name = o.Name, Value = o.Value }));
            _puller.SetIndex(_indexModel);
            eventAggregator.GetEvent<OpenIndexPreviewPageEvent>().Publish(new OpenIndexPreviewPageEventArgument
            {
                IndexModel = _indexModel,
                Puller = _puller
            });
            //var res = puller.Preview();
        }

        private IEnumerable<OptionItem> GetOptionItems()
        {
            var options = new List<OptionItem>();
            options.AddRange(PullerOptions?.Select(o => new OptionItem
            {
                Name = o.Name,
                Value = o.Value,
                OptionGroupNames = o.OptionGroupNames
            }) ?? new List<OptionItem>());
            options.AddRange(IndexerOptions?.Select(o => new OptionItem
            {
                Name = o.Name,
                Value = o.Value,
                OptionGroupNames = o.OptionGroupNames
            }) ?? new List<OptionItem>());
            options.AddRange(PusherOptions?.Select(o => new OptionItem
            {
                Name = o.Name,
                Value = o.Value,
                OptionGroupNames = o.OptionGroupNames
            }) ?? new List<OptionItem>());
            options.AddRange(MapperOptions?.Select(o => new OptionItem
            {
                Name = o.Name,
                Value = o.Value,
                OptionGroupNames = o.OptionGroupNames
            }) ?? new List<OptionItem>());
            return options;
        }

        private IEnumerable<DependencyItemModel> GetDependencies()
        {
            var dependencies = new List<DependencyItemModel>();
            dependencies.AddRange(EntityDependencyViewModel.Dependencies.Select(d => new DependencyItemModel
            {
                DependOnStep = d.DependOnStep,
                ExecuteImmediately = d.ExecuteImmediately,
                StepToExecute = d.StepToExecute,
                TargetEntityId = d.TargetEntityId,
                TargetEntityType = d.TargetEntityType,
                ForeignKeys = d.ForeignKeys,
                ReferenceKeys = d.ReferenceKeys
            }));
            dependencies.AddRange(AttributeDependencyViewModel.Dependencies.Select(d => new DependencyItemModel
            {
                DependOnStep = d.DependOnStep,
                ExecuteImmediately = d.ExecuteImmediately,
                StepToExecute = d.StepToExecute,
                TargetEntityId = d.TargetEntityId,
                TargetEntityType = d.TargetEntityType,
                ForeignKeys = d.ForeignKeys,
                ReferenceKeys = d.ReferenceKeys
            }));
            return dependencies;
        }

        private bool Save(out string message)
        {
            if (_indexModel == null)
            {
                message = "No item to save";
                return true;
            }

            BaseRepository repo = null;
            switch (_indexType)
            {
                case EntityType.Entity:
                    repo = entityRepository;
                    break;
                case EntityType.Attribute:
                    repo = attributeRepository;
                    break;
                default:
                    throw new NotSupportedException($"Index Type {_indexType} is not supported");
            }

            try
            {
                if (string.IsNullOrWhiteSpace(_indexModel?.SourceViewName))
                {
                    // max to 128 characters, luckily we use SQL Server
                    var strippedName = Regex.Replace(Name, @"[^0-9a-zA-Z_]+", "");
                    strippedName = strippedName.Length > 90 ? strippedName.Substring(0, 90) : strippedName;
                    var randomStr = StringExtensions.StringExtensions.Random(10);
                    SourceViewName = $"vw_e_{strippedName}_{randomStr}";
                    ValueTableName = $"tbl_e_{strippedName}_{randomStr}";
                    OldValueTableName = $"tbl_e_{strippedName}_{randomStr}_old";
                    NewValueTableName = $"tbl_e_{strippedName}_{randomStr}_new";
                }
                repo.BeginTransaction();
                if (_indexType == EntityType.Entity)
                {
                    repo.Update<EntityModel>(_indexModel.Id.ToString(), new
                    {
                        Name,
                        Description,
                        State = 0,
                        SourceConnectionId = SelectedSourceConnection.Id,
                        DestinationConnectionId = SelectedDestinationConnection.Id,
                        SourceProcessorId = SelectedSourceProcessor.Id,
                        DestinationProcessorId = SelectedDestinationProcessor.Id,
                        SourceViewName = SourceViewName,
                        ValueTableName = ValueTableName,
                        OldValueTableName = OldValueTableName,
                        NewValueTableName = NewValueTableName
                    });

                }
                else
                {
                    repo.Update<AttributeModel>(_indexModel.Id.ToString(), new
                    {
                        Name,
                        Description,
                        State = 0,
                        EntityId = SelectedEntity.Id,
                        SourceConnectionId = SelectedSourceConnection.Id,
                        DestinationConnectionId = SelectedDestinationConnection.Id,
                        SourceProcessorId = SelectedSourceProcessor.Id,
                        DestinationProcessorId = SelectedDestinationProcessor.Id,
                        SourceViewName = SourceViewName,
                        ValueTableName = ValueTableName,
                        OldValueTableName = OldValueTableName,
                        NewValueTableName = NewValueTableName
                    });
                }

                repo.LinkOptions(_indexModel.Id.ToString(), _indexModel.EntityType, GetOptionItems());
                repo.SetDependencies(_indexModel.Id, _indexModel.EntityType, GetDependencies());
                repo.SetTransformations(_indexModel.Id, _indexModel.EntityType, TransformationConfigureViewModel.Transformations.Select(t => t.GetModel()));
                repo.LinkOptions(_indexModel.Id.ToString(), _indexModel.EntityType, TransformationConfigureViewModel.GetTransformationOptions());

                repo.Commit();

                eventAggregator.GetEvent<RefreshIndexesListViewEvent>().Publish(new RefreshIndexesListViewEventArgument
                {
                    SelectedIndexId = _indexModel.Id.ToString(),
                    SelectedIndexType = _indexType
                });
            }
            catch
            {
                repo.RollBack();
                throw;
            }

            message = "Success";
            return true;
        }

        private bool New(out string message)
        {
            BaseRepository repo = null;
            switch (_indexType)
            {
                case EntityType.Entity:
                    repo = entityRepository;
                    break;
                case EntityType.Attribute:
                    repo = attributeRepository;
                    break;
                default:
                    throw new NotSupportedException($"Index Type {_indexType} is not supported");
            }
            try
            {
                var strippedName = Regex.Replace(Name, @"[^0-9a-zA-Z_]+", "");
                strippedName = strippedName.Length > 90 ? strippedName.Substring(0, 90) : strippedName;
                var randomStr = StringExtensions.StringExtensions.Random(10);
                var sourceViewName = $"vw_e_{strippedName}_{randomStr}";
                var valueTableName = $"tbl_e_{strippedName}_{randomStr}";
                var oldValueTableName = $"tbl_e_{strippedName}_{randomStr}_old";
                var newValueTableName = $"tbl_e_{strippedName}_{randomStr}_new";

                repo.BeginTransaction();
                var indexId = string.Empty;
                if (_indexType == EntityType.Entity)
                {
                    indexId = repo.Create<EntityModel>(new
                    {
                        Name,
                        Description,
                        State = 0,
                        SourceConnectionId = SelectedSourceConnection.Id,
                        DestinationConnectionId = SelectedDestinationConnection.Id,
                        SourceProcessorId = SelectedSourceProcessor.Id,
                        DestinationProcessorId = SelectedDestinationProcessor.Id,
                        SourceViewName = sourceViewName,
                        ValueTableName = valueTableName,
                        OldValueTableName = oldValueTableName,
                        NewValueTableName = newValueTableName
                    });
                }
                else
                {
                    indexId = repo.Create<AttributeModel>(new
                    {
                        Name,
                        Description,
                        State = 0,
                        EntityId = SelectedEntity.Id,
                        SourceConnectionId = SelectedSourceConnection.Id,
                        DestinationConnectionId = SelectedDestinationConnection.Id,
                        SourceProcessorId = SelectedSourceProcessor.Id,
                        DestinationProcessorId = SelectedDestinationProcessor.Id,
                        SourceViewName = sourceViewName,
                        ValueTableName = valueTableName,
                        OldValueTableName = oldValueTableName,
                        NewValueTableName = newValueTableName
                    });
                }

                repo.LinkOptions(indexId, _indexType, GetOptionItems());
                repo.SetDependencies(Guid.Parse(indexId), _indexType, GetDependencies());
                repo.SetTransformations(Guid.Parse(indexId), _indexType, TransformationConfigureViewModel.Transformations.Select(t => t.GetModel()));
                repo.LinkOptions(indexId, _indexType, TransformationConfigureViewModel.GetTransformationOptions());
                repo.Commit();

                eventAggregator.GetEvent<RefreshIndexesListViewEvent>().Publish(new RefreshIndexesListViewEventArgument
                {
                    SelectedIndexId = indexId,
                    SelectedIndexType = _indexType
                });
            }
            catch
            {
                repo.RollBack();
                throw;
            }
            message = "Success";
            return true;
        }

        private bool Delete(out string message)
        {
            if (_indexModel == null)
            {
                message = "No item to delete";
                return true;
            }
            BaseRepository repo = null;
            switch (_indexType)
            {
                case EntityType.Entity:
                    repo = entityRepository;
                    break;
                case EntityType.Attribute:
                    repo = attributeRepository;
                    break;
                default:
                    throw new NotSupportedException($"Index Type {_indexType} is not supported");
            }
            try
            {
                repo.BeginTransaction();
                if (_indexType == EntityType.Entity)
                {
                    repo.DeleteById<EntityModel>(_indexModel.Id.ToString());
                } else
                {
                    repo.DeleteById<AttributeModel>(_indexModel.Id.ToString());
                }
                
                repo.UnlinkOptions(_indexModel.Id.ToString());
                repo.RemoveDependencies(_indexModel.Id);
                repo.RemoveTransformations(_indexModel.Id);
                repo.Commit();

                eventAggregator.GetEvent<RefreshIndexesListViewEvent>().Publish(new RefreshIndexesListViewEventArgument
                {
                    SelectedIndexId = string.Empty,
                    SelectedIndexType = _indexType
                });
                message = "Success";
                return true;
            }
            catch
            {
                entityRepository.RollBack();
                throw;
            }
        }

        private void Manage()
        {
            if (_indexModel == null)
            {
                MessageBox.Show(
                    Application.Current.MainWindow,
                    "The entity should be created first.",
                    "Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            eventAggregator.GetEvent<OpenManageIndexPageEvent>()
                .Publish(new OpenManageIndexPageEventArgument
                {
                    IndexModel = _indexModel,
                    Puller = _puller,
                    Indexer = _indexer,
                    Pusher = _pusher,
                    Mapper = _mapper
                });
        }

        public void SetIndexType(EntityType indexType)
        {
            _indexType = indexType;
            IsAttribute = true;
            EntityDependencyViewModel.SetTargetType(indexType);
            AttributeDependencyViewModel.SetTargetType(indexType);
        }
        
        public void Loaded()
        {
            //throw new NotImplementedException();
        }

        private void OnIndexesChanged(RefreshIndexesListViewEventArgument args)
        {
            Entities = new ObservableCollection<EntityModel>(entityRepository.GetAll());
        }

        private void OnConnectionsChanged(RefreshConnectionListEventArgument args)
        {
            SourceConnections = new ObservableCollection<ConnectionModel>(connectionRepository.GetAll());
            DestinationConnections = new ObservableCollection<ConnectionModel>(connectionRepository.GetAll());
        }

        public void LoadOptions()
        {
            var options = _indexModel != null 
                ? entityRepository.LoadOptions(_indexModel?.Id.ToString(), _indexModel.EntityType)
                : new List<OptionModel>();
            var optionItems = options?.Select(o => new OptionItem { Name = o.Key, Value = o.Value }) ?? new List<OptionItem>();
            LoadPuller(optionItems);
            LoadPusher(optionItems);
            LoadIndexer(optionItems);
            LoadMapper(optionItems);
        }

        private void LoadIndexer(IEnumerable<OptionItem> options)
        {
            switch (_indexType)
            {
                case EntityType.Entity:
                    _indexer = indexers.Where(p => typeof(IEntityIndexer).IsAssignableFrom(p.GetType()))
                        .Select(p => (IEntityIndexer)p)
                        .FirstOrDefault(p => p.IsImplemented(
                            SelectedSourceProcessor?.Id,
                            SelectedSourceConnection?.ProviderId));
                    break;
                case EntityType.Attribute:
                    _indexer = indexers.Where(p => typeof(IAttributeIndexer).IsAssignableFrom(p.GetType()))
                        .Select(p => (IAttributeIndexer)p)
                        .FirstOrDefault(p => p.IsImplemented(
                            SelectedSourceProcessor?.Id,
                            SelectedEntity?.SourceProcessorId,
                            SelectedSourceConnection?.ProviderId));
                    break;
                default:
                    return;
            }

            _indexer?.SetOptions(options);
            IndexerOptions = new ObservableCollection<OptionItemViewModel>(_indexer?.Options.Select(o =>
            {
                var result = new OptionItemViewModel();
                result.SetOption(o);
                return result;
            }) ?? new List<OptionItemViewModel>());
        }

        private void LoadPusher(IEnumerable<OptionItem> options)
        {
            switch (_indexType)
            {
                case EntityType.Entity:
                    _pusher = pushers.Where(p => typeof(IEntityPusher).IsAssignableFrom(p.GetType()))
                        .Select(p => (IEntityPusher)p)
                        .FirstOrDefault(p => p.IsImplemented(
                            SelectedDestinationProcessor?.Id,
                            SelectedDestinationConnection?.ProviderId));
                    break;
                case EntityType.Attribute:
                    _pusher = pushers.Where(p => typeof(IAttributePusher).IsAssignableFrom(p.GetType()))
                        .Select(p => (IAttributePusher)p)
                        .FirstOrDefault(p => p.IsImplemented(
                            SelectedDestinationProcessor?.Id,
                            SelectedEntity?.DestinationProcessorId,
                            SelectedDestinationConnection?.ProviderId));
                    break;
                default:
                    return;
            }

            _pusher?.SetOptions(options);
            PusherOptions = new ObservableCollection<OptionItemViewModel>(_pusher?.Options.Select(o =>
            {
                var result = new OptionItemViewModel();
                result.SetOption(o);
                return result;
            }) ?? new List<OptionItemViewModel>());
        }

        private void LoadMapper(IEnumerable<OptionItem> options)
        {
            switch (_indexType)
            {
                case EntityType.Entity:
                    _mapper = mappers.Where(p => typeof(IMapper).IsAssignableFrom(p.GetType()))
                        .FirstOrDefault(p => p.IsImplemented(
                            SelectedDestinationProcessor?.Id,
                            SelectedDestinationConnection?.ProviderId));
                    break;
                default:
                    return;
            }

            _mapper?.SetOptions(options);
            MapperOptions = new ObservableCollection<OptionItemViewModel>(_mapper?.Options.Select(o =>
            {
                var result = new OptionItemViewModel();
                result.SetOption(o);
                return result;
            }) ?? new List<OptionItemViewModel>());
        }

        private void LoadPuller(IEnumerable<OptionItem> options)
        {
            switch (_indexType)
            {
                case EntityType.Entity:
                    _puller = pullers.Where(p => typeof(IEntityPuller).IsAssignableFrom(p.GetType()))
                        .Select(p => (IEntityPuller)p)
                        .FirstOrDefault(p => p.IsImplemented(
                            SelectedSourceProcessor?.Id,
                            SelectedSourceConnection?.ProviderId));
                    break;
                case EntityType.Attribute:
                    _puller = pullers.Where(p => typeof(IAttributePuller).IsAssignableFrom(p.GetType()))
                        .Select(p => (IAttributePuller)p)
                        .FirstOrDefault(p => p.IsImplemented(
                            SelectedSourceProcessor?.Id,
                            SelectedEntity?.SourceProcessorId,
                            SelectedSourceConnection?.ProviderId));
                    break;
                default:
                    return;
            }

            _puller?.SetOptions(options);
            PullerOptions = new ObservableCollection<OptionItemViewModel>(_puller?.Options.Select(o =>
            {
                var result = new OptionItemViewModel();
                result.SetOption(o);
                return result;
            }) ?? new List<OptionItemViewModel>());
        }
    }
}
