using FastSQL.App.Events;
using FastSQL.App.Interfaces;
using FastSQL.App.UserControls.Dependencies;
using FastSQL.App.UserControls.Transformers;
using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FastSQL.App.UserControls.Entities
{
    public class EntityContentViewModel : BaseViewModel
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IEntityIndexer indexer;
        private readonly IEnumerable<IEntityPuller> pullers;
        private readonly IEnumerable<IEntityPusher> pushers;
        private readonly IEnumerable<ITransformer> transformers;
        
        private EntityModel _entity;
        private readonly EntityRepository entityRepository;
        private readonly ConnectionRepository connectionRepository;
        private ConnectionModel _sourceConnection;
        private ConnectionModel _destinationConnection;
        private IProcessor _sourceProcessor;
        private IProcessor _destinationProcessor;

        private ObservableCollection<string> _commands;
        private ObservableCollection<OptionItemViewModel> _pullerOptions;
        private ObservableCollection<OptionItemViewModel> _indexerOptions;
        private ObservableCollection<OptionItemViewModel> _pusherOptions;
        private ObservableCollection<ConnectionModel> _sourceConnections;
        private ObservableCollection<IProcessor> _sourceProcessors;
        private ObservableCollection<ConnectionModel> _destinationConnections;
        private ObservableCollection<IProcessor> _destinationProcessors;

        private EntityDependencyViewModel _entityDependencyViewModel;
        private AttributeDependencyViewModel _attributeDependencyViewModel;
        private UCTransformationConfigureViewModel _transformationConfigureViewModel;

        public BaseCommand ApplyCommand => new BaseCommand(o => true, OnApplyCommand);

        public string Name
        {
            get => _entity?.Name;
            set
            {
                if (_entity != null)
                {
                    _entity.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Description
        {
            get => _entity?.Description;
            set
            {
                if (_entity != null)
                {
                    _entity.Description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public bool Enabled
        {
            get => _entity?.HasState(EntityState.Disabled) == true ? false : true;
            set
            {
                if (_entity != null)
                {
                    if (value)
                    {
                        _entity.RemoveState(EntityState.Disabled);
                    }
                    else
                    {
                        _entity.AddState(EntityState.Disabled);
                    }
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public ConnectionModel SelectedSourceConnection
        {
            get => _sourceConnection;
            set
            {
                _sourceConnection = value;
                LoadOptions();
                OnPropertyChanged(nameof(SelectedSourceConnection));
            }
        }

        public ConnectionModel SelectedDestinationConnection
        {
            get => _destinationConnection;
            set
            {
                _destinationConnection = value;
                LoadOptions();
                OnPropertyChanged(nameof(SelectedDestinationConnection));
            }
        }
        
        public IProcessor SelectedSourceProcessor
        {
            get => _sourceProcessor;
            set
            {
                _sourceProcessor = value;
                LoadOptions();
                OnPropertyChanged(nameof(SelectedSourceProcessor));
            }
        }

        public IProcessor SelectedDestinationProcessor
        {
            get => _destinationProcessor;
            set
            {
                _destinationProcessor = value;
                LoadOptions();
                OnPropertyChanged(nameof(SelectedDestinationProcessor));
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

        public EntityDependencyViewModel EntityDependencyViewModel
        {
            get => _entityDependencyViewModel;
            set
            {
                _entityDependencyViewModel = value;
                OnPropertyChanged(nameof(EntityDependencyViewModel));
            }
        }

        public AttributeDependencyViewModel AttributeDependencyViewModel
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

        public EntityContentViewModel(
            IEventAggregator eventAggregator,
            IEnumerable<IProcessor> processors,
            IEnumerable<IEntityPuller> pullers,
            IEntityIndexer indexer,
            IEnumerable<IEntityPusher> pushers,
            IEnumerable<ITransformer> transformers,
            EntityRepository entityRepository,
            ConnectionRepository connectionRepository,
            EntityDependencyViewModel entityDependencyViewModel,
            AttributeDependencyViewModel attributeDependencyViewModel,
            UCTransformationConfigureViewModel transformationConfigureViewModel)
        {
            this.eventAggregator = eventAggregator;
            this.entityRepository = entityRepository;
            this.connectionRepository = connectionRepository;

            this.pullers = pullers;
            this.indexer = indexer;
            this.pushers = pushers;
            this.transformers = transformers;

            EntityDependencyViewModel = entityDependencyViewModel;
            AttributeDependencyViewModel = attributeDependencyViewModel;
            TransformationConfigureViewModel = transformationConfigureViewModel;

            // Need to duplication code here, weird behavior of WPF
            SourceProcessors = new ObservableCollection<IProcessor>(processors.Where(p => p.Type == ProcessorType.Entity));
            SourceConnections = new ObservableCollection<ConnectionModel>(connectionRepository.GetAll());

            DestinationProcessors = new ObservableCollection<IProcessor>(processors.Where(p => p.Type == ProcessorType.Entity));
            DestinationConnections = new ObservableCollection<ConnectionModel>(connectionRepository.GetAll());

            LoadOptions();

            Commands = new ObservableCollection<string>(new List<string> { "Save", "New", "Delete", "Preview", "Manage" });
            eventAggregator.GetEvent<SelectEntityEvent>().Subscribe(OnSelectEntity);
        }

        private void LoadOptions()
        {
            var puller = pullers.FirstOrDefault(p =>
                !string.IsNullOrWhiteSpace(SelectedSourceProcessor?.Id)
                && !string.IsNullOrWhiteSpace(SelectedSourceConnection?.Id.ToString())
                && p.IsImplemented(SelectedSourceProcessor.Id, SelectedSourceConnection.ProviderId));
            var pusher = pushers.FirstOrDefault(p =>
                !string.IsNullOrWhiteSpace(SelectedDestinationProcessor?.Id)
                && !string.IsNullOrWhiteSpace(SelectedDestinationConnection?.Id.ToString())
                && p.IsImplemented(SelectedDestinationProcessor.Id, SelectedDestinationConnection.ProviderId));
            IEnumerable<OptionModel> options = new List<OptionModel>();
            if (_entity != null)
            {
                options = entityRepository.LoadOptions(_entity.Id) ?? new List<OptionModel>();
            }
            var optionItems = options.Select(o => new OptionItem { Name = o.Key, Value = o.Value });

            // They will automatically filter their own options
            puller?.SetOptions(optionItems);
            indexer?.SetOptions(optionItems);
            pusher?.SetOptions(optionItems);

            PullerOptions = new ObservableCollection<OptionItemViewModel>(puller?.Options.Select(o =>
            {
                var result = new OptionItemViewModel();
                result.SetOption(o);
                return result;
            }) ?? new List<OptionItemViewModel>());

            IndexerOptions = new ObservableCollection<OptionItemViewModel>(indexer?.Options.Select(o =>
            {
                var result = new OptionItemViewModel();
                result.SetOption(o);
                return result;
            }) ?? new List<OptionItemViewModel>());

            PusherOptions = new ObservableCollection<OptionItemViewModel>(pusher?.Options.Select(o =>
            {
                var result = new OptionItemViewModel();
                result.SetOption(o);
                return result;
            }) ?? new List<OptionItemViewModel>());
        }

        private void OnSelectEntity(SelectEntityEventArgument obj)
        {
            var entity = entityRepository.GetById(obj.EntityId);
            _entity = entity;

            Name = entity.Name;
            Description = entity.Description;
            Enabled = !entity.HasState(EntityState.Disabled);

            SelectedSourceConnection = SourceConnections.FirstOrDefault(c => c.Id == entity.SourceConnectionId);
            SelectedDestinationConnection = DestinationConnections.FirstOrDefault(c => c.Id == entity.DestinationConnectionId);

            SelectedSourceProcessor = SourceProcessors.FirstOrDefault(p => p.Id == entity.SourceProcessorId);
            SelectedDestinationProcessor = DestinationProcessors.FirstOrDefault(p => p.Id == entity.DestinationProcessorId);

            EntityDependencyViewModel.SetEntity(entity);
            AttributeDependencyViewModel.SetEntity(entity);
            TransformationConfigureViewModel.SetEntity(entity);
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
                Application.Current.MainWindow,
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
                  Application.Current.MainWindow,
                  $"Missing basic configuration for source",
                  "Failed",
                  MessageBoxButton.OK,
                  MessageBoxImage.Error);
                return;
            }

            var puller = pullers.FirstOrDefault(p => p.IsImplemented(SelectedSourceProcessor.Id, SelectedSourceConnection.ProviderId));
            if (puller == null)
            {
                MessageBox.Show(
                  Application.Current.MainWindow,
                  $"Could not find any adapter that implements {SelectedSourceConnection.Name}/{SelectedSourceProcessor.Name}",
                  "Failed",
                  MessageBoxButton.OK,
                  MessageBoxImage.Error);
                return;
            }

            puller.SetOptions(PullerOptions.Select(o => new OptionItem { Name = o.Name, Value = o.Value }));
            puller.SetEntity(_entity);
            eventAggregator.GetEvent<EntityPreviewPageEvent>().Publish(new EntityPreviewPageEventArgument {
                Puller = puller,
                Entity = _entity
            });
            //var res = puller.Preview();
        }

        private IEnumerable<OptionItem> GetOptionItems()
        {
            var options = new List<OptionItem>();
            options.AddRange(PullerOptions.Select(o => new OptionItem
            {
                Name = o.Name,
                Value = o.Value,
                OptionGroupNames = o.OptionGroupNames
            }));
            options.AddRange(IndexerOptions.Select(o => new OptionItem
            {
                Name = o.Name,
                Value = o.Value,
                OptionGroupNames = o.OptionGroupNames
            }));
            options.AddRange(PusherOptions.Select(o => new OptionItem
            {
                Name = o.Name,
                Value = o.Value,
                OptionGroupNames = o.OptionGroupNames
            }));
            return options;
        }

        private IEnumerable<DependencyItemModel> GetDependencies(Guid entityId)
        {
            var dependencies = new List<DependencyItemModel>();
            dependencies.AddRange(EntityDependencyViewModel.Dependencies.Select(d => new DependencyItemModel
            {
                EntityId = entityId,
                EntityType = EntityType.Entity,
                DependOnStep = d.DependOnStep,
                ExecuteImmediately = d.ExecuteImmediately,
                StepToExecute = d.StepToExecute,
                TargetEntityId = d.TargetEntityId,
                TargetEntityType = d.TargetEntityType
            }));
            dependencies.AddRange(AttributeDependencyViewModel.Dependencies.Select(d => new DependencyItemModel
            {
                EntityId = entityId,
                EntityType = EntityType.Entity,
                DependOnStep = d.DependOnStep,
                ExecuteImmediately = d.ExecuteImmediately,
                StepToExecute = d.StepToExecute,
                TargetEntityId = d.TargetEntityId,
                TargetEntityType = d.TargetEntityType
            }));
            return dependencies;
        }

        private bool Save(out string message)
        {
            if (_entity == null)
            {
                message = "No item to save";
                return true;
            }

            try
            {
                entityRepository.BeginTransaction();
                var result = entityRepository.Update(_entity.Id.ToString(), new
                {
                    Name,
                    Description,
                    _entity.State,
                    SourceConnectionId = SelectedSourceConnection.Id,
                    DestinationConnectionId = SelectedDestinationConnection.Id,
                    SourceProcessorId = SelectedSourceProcessor.Id,
                    DestinationProcessorId = SelectedDestinationProcessor.Id,
                });

                entityRepository.LinkOptions(_entity.Id, GetOptionItems());
                entityRepository.SetDependencies(_entity.Id, GetDependencies(_entity.Id));
                entityRepository.SetTransformations(_entity.Id, TransformationConfigureViewModel.Transformations.Select(t => t.GetModel()));
                entityRepository.LinkOptions(_entity.Id, TransformationConfigureViewModel.GetTransformationOptions());

                entityRepository.Commit();
            }
            catch
            {
                entityRepository.RollBack();
                throw;
            }

            message = "Success";
            return true;
        }

        private bool New(out string message)
        {
            try
            {
                entityRepository.BeginTransaction();
                var entityId = entityRepository.Create(new
                {
                    Name,
                    Description,
                    State = 0,
                    SourceConnectionId = SelectedSourceConnection.Id,
                    DestinationConnectionId = SelectedDestinationConnection.Id,
                    SourceProcessorId = SelectedSourceProcessor.Id,
                    DestinationProcessorId = SelectedDestinationProcessor.Id,
                });
                var entityIdGuid = Guid.Parse(entityId);

                entityRepository.LinkOptions(entityIdGuid, GetOptionItems());
                entityRepository.SetDependencies(entityIdGuid, GetDependencies(entityIdGuid));
                entityRepository.SetTransformations(entityIdGuid, TransformationConfigureViewModel.Transformations.Select(t => t.GetModel()));
                entityRepository.LinkOptions(entityIdGuid, TransformationConfigureViewModel.GetTransformationOptions());
                entityRepository.Commit();

                eventAggregator.GetEvent<RefreshEntityListEvent>().Publish(new RefreshEntityListEventArgument
                {
                    SelectedEntityId = entityId
                });
            }
            catch
            {
                entityRepository.RollBack();
                throw;
            }

            message = "Success";
            return true;
        }

        private bool Delete(out string message)
        {
            if (_entity == null)
            {
                message = "No item to delete";
                return true;
            }
            try
            {
                entityRepository.BeginTransaction();
                entityRepository.DeleteById(_entity.Id.ToString());
                entityRepository.UnlinkOptions(_entity.Id);
                entityRepository.RemoveDependencies(_entity.Id);
                entityRepository.RemoveTransformations(_entity.Id);
                entityRepository.Commit();

                eventAggregator.GetEvent<RefreshEntityListEvent>().Publish(new RefreshEntityListEventArgument
                {
                    SelectedEntityId = string.Empty
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
            if (_entity == null)
            {
                MessageBox.Show(
                    Application.Current.MainWindow,
                    "The entity should be created first.",
                    "Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            eventAggregator.GetEvent<OpenManageEntityPageEvent>()
                .Publish(new OpenManageEntityPageEventArgument
                {
                    Entity = _entity
                });
        }
    }
}
