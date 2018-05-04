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

namespace FastSQL.App.UserControls.Attributes
{
    public class AttributeContentViewModel : BaseViewModel
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IEnumerable<IProcessor> processors;
        private readonly IAttributeIndexer indexer;
        private readonly IEnumerable<IAttributePuller> pullers;
        private readonly IEnumerable<IAttributePusher> pushers;
        private readonly IEnumerable<ITransformer> transformers;

        private AttributeModel _attribute;
        private EntityModel _entity;
        private readonly AttributeRepository attributeRepository;
        private readonly EntityRepository entityRepository;
        private readonly ConnectionRepository connectionRepository;
        private ConnectionModel _sourceConnection;
        private ConnectionModel _destinationConnection;
        private IProcessor _sourceProcessor;
        private IProcessor _destinationProcessor;

        private ObservableCollection<OptionItemViewModel> _pullerOptions;
        private ObservableCollection<OptionItemViewModel> _indexerOptions;
        private ObservableCollection<OptionItemViewModel> _pusherOptions;
        private ObservableCollection<string> _commands;
        private ObservableCollection<ConnectionModel> _sourceConnections;
        private ObservableCollection<ConnectionModel> _destinationConnections;
        private ObservableCollection<IProcessor> _sourceProcessors;
        private ObservableCollection<IProcessor> _destinationProcessors;
        private ObservableCollection<EntityModel> _entities;

        private EntityDependencyViewModel _entityDependencyViewModel;
        private AttributeDependencyViewModel _attributeDependencyViewModel;
        private UCTransformationConfigureViewModel _transformationConfigureViewModel;

        public BaseCommand ApplyCommand => new BaseCommand(o => true, OnApplyCommand);

        public string Name
        {
            get => _attribute?.Name;
            set
            {
                if (_attribute != null)
                {
                    _attribute.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Description
        {
            get => _attribute?.Description;
            set
            {
                if (_attribute != null)
                {
                    _attribute.Description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public bool Enabled
        {
            get => _attribute?.HasState(EntityState.Disabled) == true ? false : true;
            set
            {
                if (_attribute != null)
                {
                    if (value)
                    {
                        _attribute.RemoveState(EntityState.Disabled);
                    }
                    else
                    {
                        _attribute.AddState(EntityState.Disabled);
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

        public EntityModel SelectedEntity
        {
            get => _entity;
            set
            {
                _entity = value;
                LoadOptions();
                OnPropertyChanged(nameof(SelectedEntity));
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

        public ObservableCollection<EntityModel> Entities
        {
            get
            {
                return _entities;
            }
            set
            {
                _entities = value;
                OnPropertyChanged(nameof(Entities));
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

        public AttributeContentViewModel(
            IEventAggregator eventAggregator,
            IEnumerable<IProcessor> processors,
            IEnumerable<IAttributePuller> pullers,
            IAttributeIndexer indexer,
            IEnumerable<IAttributePusher> pushers,
            IEnumerable<ITransformer> transformers,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            ConnectionRepository connectionRepository,
            EntityDependencyViewModel entityDependencyViewModel,
            AttributeDependencyViewModel attributeDependencyViewModel,
            UCTransformationConfigureViewModel transformationConfigureViewModel)
        {
            this.eventAggregator = eventAggregator;
            this.processors = processors;
            this.entityRepository = entityRepository;
            this.attributeRepository = attributeRepository;
            this.connectionRepository = connectionRepository;

            this.pullers = pullers;
            this.indexer = indexer;
            this.pushers = pushers;
            this.transformers = transformers;

            EntityDependencyViewModel = entityDependencyViewModel;
            AttributeDependencyViewModel = attributeDependencyViewModel;
            TransformationConfigureViewModel = transformationConfigureViewModel;

            // Need to duplication code here, weird behavior of WPF
            SourceProcessors = new ObservableCollection<IProcessor>(processors.Where(p => p.Type == ProcessorType.Attribute));
            SourceConnections = new ObservableCollection<ConnectionModel>(connectionRepository.GetAll());

            DestinationProcessors = new ObservableCollection<IProcessor>(processors.Where(p => p.Type == ProcessorType.Attribute));
            DestinationConnections = new ObservableCollection<ConnectionModel>(connectionRepository.GetAll());

            Entities = new ObservableCollection<EntityModel>(entityRepository.GetAll());

            LoadOptions();

            Commands = new ObservableCollection<string>(new List<string> { "Save", "New", "Delete", "Preview", "Manage" });
            eventAggregator.GetEvent<SelectAttributeEvent>().Subscribe(OnSelectAttribute);
        }

        private void LoadOptions()
        {
            IPuller puller = null;
            IPusher pusher = null;
            if (SelectedEntity != null)
            {
                var entitySourceProcessor = processors.FirstOrDefault(p => p.Id == SelectedEntity.SourceProcessorId);
                var entityDestinationProcessor = processors.FirstOrDefault(p => p.Id == SelectedEntity.DestinationProcessorId);
                puller = pullers.FirstOrDefault(p =>
                !string.IsNullOrWhiteSpace(SelectedSourceProcessor?.Id)
                && !string.IsNullOrWhiteSpace(SelectedSourceConnection?.Id.ToString())
                && p.IsImplemented(SelectedSourceProcessor.Id, entitySourceProcessor.Id, SelectedSourceConnection.ProviderId));
                pusher = pushers.FirstOrDefault(p =>
                    !string.IsNullOrWhiteSpace(SelectedDestinationProcessor?.Id)
                    && !string.IsNullOrWhiteSpace(SelectedDestinationConnection?.Id.ToString())
                    && p.IsImplemented(SelectedDestinationProcessor.Id, entityDestinationProcessor.Id, SelectedDestinationConnection.ProviderId));
            }

            IEnumerable<OptionModel> options = new List<OptionModel>();
            if (_attribute != null)
            {
                options = attributeRepository.LoadOptions(_attribute.Id.ToString()) ?? new List<OptionModel>();
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

        private void OnSelectAttribute(SelectAttributeEventArgument obj)
        {
            var attr = attributeRepository.GetById(obj.AttributeId);
            _attribute = attr;
            SelectedEntity = Entities.FirstOrDefault(e => e.Id == attr.EntityId);

            Name = attr.Name;
            Description = attr.Description;
            Enabled = !attr.HasState(EntityState.Disabled);

            SelectedSourceConnection = SourceConnections.FirstOrDefault(c => c.Id == attr.SourceConnectionId);
            SelectedDestinationConnection = DestinationConnections.FirstOrDefault(c => c.Id == attr.DestinationConnectionId);

            SelectedSourceProcessor = SourceProcessors.FirstOrDefault(p => p.Id == attr.SourceProcessorId);
            SelectedDestinationProcessor = DestinationProcessors.FirstOrDefault(p => p.Id == attr.DestinationProcessorId);

            EntityDependencyViewModel.SetIndex(attr);
            AttributeDependencyViewModel.SetIndex(attr);
            TransformationConfigureViewModel.SetEntity(attr);
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

        private IEnumerable<DependencyItemModel> GetDependencies(Guid attributeId)
        {
            var dependencies = new List<DependencyItemModel>();
            dependencies.AddRange(EntityDependencyViewModel.Dependencies.Select(d => new DependencyItemModel
            {
                EntityId = attributeId,
                EntityType = EntityType.Attribute,
                DependOnStep = d.DependOnStep,
                ExecuteImmediately = d.ExecuteImmediately,
                StepToExecute = d.StepToExecute,
                TargetEntityId = d.TargetEntityId,
                TargetEntityType = d.TargetEntityType
            }));
            dependencies.AddRange(AttributeDependencyViewModel.Dependencies.Select(d => new DependencyItemModel
            {
                EntityId = attributeId,
                EntityType = EntityType.Attribute,
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
            if (_attribute == null)
            {
                message = "No item to save";
                return true;
            }

            try
            {
                attributeRepository.BeginTransaction();
                var result = attributeRepository.Update(_attribute.Id.ToString(), new
                {
                    Name,
                    Description,
                    _attribute.State,
                    EntityId = _entity?.Id,
                    SourceConnectionId = SelectedSourceConnection.Id,
                    DestinationConnectionId = SelectedDestinationConnection.Id,
                    SourceProcessorId = SelectedSourceProcessor.Id,
                    DestinationProcessorId = SelectedDestinationProcessor.Id,
                });

                attributeRepository.LinkOptions(_attribute.Id.ToString(), GetOptionItems());

                attributeRepository.SetDependencies(_attribute.Id, GetDependencies(_attribute.Id));

                attributeRepository.SetTransformations(_attribute.Id, TransformationConfigureViewModel.Transformations.Select(t => t.GetModel()));
                attributeRepository.LinkOptions(_attribute.Id.ToString(), TransformationConfigureViewModel.GetTransformationOptions());

                attributeRepository.Commit();
            }
            catch
            {
                attributeRepository.RollBack();
                throw;
            }

            message = "Success";
            return true;
        }

        private bool New(out string message)
        {
            try
            {
                attributeRepository.BeginTransaction();
                var attrId = attributeRepository.Create(new
                {
                    Name,
                    Description,
                    State = 0,
                    EntityId = SelectedEntity.Id,
                    SourceConnectionId = SelectedSourceConnection.Id,
                    DestinationConnectionId = SelectedDestinationConnection.Id,
                    SourceProcessorId = SelectedSourceProcessor.Id,
                    DestinationProcessorId = SelectedDestinationProcessor.Id,
                });
                var attrGuidId = Guid.Parse(attrId);

                attributeRepository.LinkOptions(attrGuidId.ToString(), GetOptionItems());
                attributeRepository.SetDependencies(attrGuidId, GetDependencies(attrGuidId));
                attributeRepository.SetTransformations(attrGuidId, TransformationConfigureViewModel.Transformations.Select(t => t.GetModel()));
                attributeRepository.LinkOptions(attrGuidId.ToString(), TransformationConfigureViewModel.GetTransformationOptions());
                attributeRepository.Commit();

                eventAggregator.GetEvent<RefreshAttributeListEvent>().Publish(new RefreshAttributeListEventArgument
                {
                    SelectedAttributeId = attrId
                });
            }
            catch
            {
                attributeRepository.RollBack();
                throw;
            }

            message = "Success";
            return true;
        }

        private bool Delete(out string message)
        {
            if (_attribute == null)
            {
                message = "No item to delete";
                return true;
            }
            try
            {
                attributeRepository.BeginTransaction();
                attributeRepository.DeleteById(_attribute.Id.ToString());
                attributeRepository.UnlinkOptions(_attribute.Id.ToString());
                attributeRepository.RemoveDependencies(_attribute.Id);
                attributeRepository.RemoveTransformations(_attribute.Id);
                attributeRepository.Commit();

                eventAggregator.GetEvent<RefreshAttributeListEvent>().Publish(new RefreshAttributeListEventArgument
                {
                    SelectedAttributeId = string.Empty
                });
                message = "Success";
                return true;
            }
            catch
            {
                attributeRepository.RollBack();
                throw;
            }
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

            if (SelectedEntity == null)
            {
                MessageBox.Show(
                  Application.Current.MainWindow,
                  $"Missing basic configuration for entity",
                  "Failed",
                  MessageBoxButton.OK,
                  MessageBoxImage.Error);
                return;
            }

            var puller = pullers.FirstOrDefault(p => p.IsImplemented(SelectedSourceProcessor.Id, SelectedEntity.SourceProcessorId, SelectedSourceConnection.ProviderId));
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
            puller.SetAttribute(_attribute, SelectedEntity);
            eventAggregator.GetEvent<AttributePreviewPageEvent>().Publish(new AttributePreviewPageEventArgument
            {
                Puller = puller,
                Entity = SelectedEntity,
                Attribute = _attribute
            });
        }

        private void Manage()
        {
            if (_attribute == null)
            {
                MessageBox.Show(
                    Application.Current.MainWindow,
                    "The attribute should be created first.",
                    "Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            eventAggregator.GetEvent<OpenManageAttributePageEvent>()
                .Publish(new OpenManageAttributePageEventArgument
                {
                    Attribute = _attribute,
                    Entity = entityRepository.GetById(_attribute.EntityId.ToString())
                });
        }
    }
}
