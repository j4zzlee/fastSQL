using FastSQL.App.Events;
using FastSQL.App.Interfaces;
using FastSQL.App.UserControls.Dependencies;
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
        private ObservableCollection<string> _commands;

        private EntityModel _entity;
        private readonly EntityRepository entityRepository;
        private readonly ConnectionRepository connectionRepository;
        private readonly IEnumerable<IEntityPuller> pullers;
        private readonly IEntityIndexer indexer;
        private readonly IEnumerable<IEntityPusher> pushers;
        private ConnectionModel _sourceConnection;
        private ConnectionModel _destinationConnection;
        private IProcessor _sourceProcessor;
        private IProcessor _destinationProcessor;

        private ObservableCollection<OptionItemViewModel> _pullerOptions;
        private ObservableCollection<OptionItemViewModel> _indexerOptions;
        private ObservableCollection<OptionItemViewModel> _pusherOptions;
        private ObservableCollection<ConnectionModel> _sourceConnections;
        private ObservableCollection<IProcessor> _sourceProcessors;
        private ObservableCollection<ConnectionModel> _destinationConnections;
        private ObservableCollection<IProcessor> _destinationProcessors;
        private EntityDependencyViewModel _entityDependencyViewModel;
        private AttributeDependencyViewModel _attributeDependencyViewModel;

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

            PullerOptions = new ObservableCollection<OptionItemViewModel>(puller?.Options.Select(o => {
                var result = new OptionItemViewModel();
                result.SetOption(o);
                return result;
            }) ?? new List<OptionItemViewModel>());

            IndexerOptions = new ObservableCollection<OptionItemViewModel>(indexer?.Options.Select(o => {
                var result = new OptionItemViewModel();
                result.SetOption(o);
                return result;
            }) ?? new List<OptionItemViewModel>());

            PusherOptions = new ObservableCollection<OptionItemViewModel>(pusher?.Options.Select(o => {
                var result = new OptionItemViewModel();
                result.SetOption(o);
                return result;
            }) ?? new List<OptionItemViewModel>());
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

        public EntityContentViewModel(
            IEventAggregator eventAggregator,
            EntityRepository entityRepository,
            ConnectionRepository connectionRepository,
            IEnumerable<IProcessor> processors,
            IEnumerable<IEntityPuller> pullers,
            IEntityIndexer indexer,
            IEnumerable<IEntityPusher> pushers,
            EntityDependencyViewModel entityDependencyViewModel,
            AttributeDependencyViewModel attributeDependencyViewModel)
        {
            this.eventAggregator = eventAggregator;
            this.entityRepository = entityRepository;
            this.connectionRepository = connectionRepository;
            this.pullers = pullers;
            this.indexer = indexer;
            this.pushers = pushers;

            EntityDependencyViewModel = entityDependencyViewModel;
            AttributeDependencyViewModel = attributeDependencyViewModel;

            // Need to duplication code here, weird behavior of WPF
            SourceProcessors = new ObservableCollection<IProcessor>(processors.Where(p => p.Type == ProcessorType.Entity));
            SourceConnections = new ObservableCollection<ConnectionModel>(connectionRepository.GetAll());

            DestinationProcessors = new ObservableCollection<IProcessor>(processors.Where(p => p.Type == ProcessorType.Entity));
            DestinationConnections = new ObservableCollection<ConnectionModel>(connectionRepository.GetAll());

            LoadOptions();

            Commands = new ObservableCollection<string>(new List<string> { "Save", "New", "Delete", "Preview", "Manage" });
            eventAggregator.GetEvent<SelectEntityEvent>().Subscribe(OnSelectEntity);
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
        }

        private void OnApplyCommand(object obj)
        {
            var commandText = obj.ToString();
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
                case "Preview":
                    success = Preview(out message);
                    break;
                case "Manage":
                    success = Manage(out message);
                    break;
            }
            MessageBox.Show(
                Application.Current.MainWindow,
                message,
                success ? "Success" : "Failed",
                MessageBoxButton.OK,
                success ? MessageBoxImage.Information : MessageBoxImage.Error);

        }

        private bool Save(out string message)
        {
            //if (_entity == null)
            //{
            //    message = "No item to save";
            //    return true;
            //}

            //try
            //{
            //    entityRepository.BeginTransaction();
            //    var result = entityRepository.Update(_entity.Id.ToString(), new
            //    {
            //        Name,
            //        Description,
            //        ProviderId = SelectedProvider.Id
            //    });

            //    SelectedProvider.SetOptions(Options?.Select(o => new OptionItem { Name = o.Name, Value = o.Value }) ?? new List<OptionItem>());

            //    entityRepository.LinkOptions(_entity.Id, SelectedProvider.Options);
            //    entityRepository.Commit();
            //}
            //catch
            //{
            //    entityRepository.RollBack();
            //    throw;
            //}

            message = "Success";
            return true;
        }

        private bool New(out string message)
        {
            //try
            //{
            //    entityRepository.BeginTransaction();
            //    var result = entityRepository.Create(new
            //    {
            //        Name,
            //        Description,
            //        ProviderId = SelectedProvider.Id
            //    });

            //    SelectedProvider.SetOptions(Options?.Select(o => new OptionItem { Name = o.Name, Value = o.Value }) ?? new List<OptionItem>());

            //    entityRepository.LinkOptions(Guid.Parse(result), SelectedProvider.Options);
            //    entityRepository.Commit();

            //    eventAggregator.GetEvent<RefreshEntityListEvent>().Publish(new RefreshEntityListEventArgument
            //    {
            //        SelectedEntityId = result
            //    });
            //}
            //catch
            //{
            //    entityRepository.RollBack();
            //    throw;
            //}

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
        
        private bool Manage(out string message)
        {
            message = "Method is not implemented";
            return false;
        }

        private bool Preview(out string message)
        {
            message = "Method is not implemented";
            return false;
        }
    }
}
