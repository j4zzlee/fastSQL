using FastSQL.App.Events;
using FastSQL.App.Interfaces;
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
    public class AttributeContentViewModel: BaseViewModel
    {
        private readonly IEventAggregator eventAggregator;
        private readonly AttributeRepository attributeRepository;
        private ObservableCollection<string> _commands;

        private AttributeModel _attribute;
        private EntityModel _entity;
        private readonly EntityRepository entityRepository;
        private readonly ConnectionRepository connectionRepository;
        private ConnectionModel _sourceConnection;
        private ConnectionModel _destinationConnection;
        private IProcessor _sourceProcessor;
        private IProcessor _destinationProcessor;

        private ObservableCollection<OptionItemViewModel> _pullerOptions;
        private ObservableCollection<OptionItemViewModel> _indexerOptions;
        private ObservableCollection<OptionItemViewModel> _pusherOptions;
        private ObservableCollection<ConnectionModel> _connections;
        private ObservableCollection<IProcessor> _processors;
        private ObservableCollection<EntityModel> _entities;

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

        public EntityModel SelectedEntity
        {
            get => _entity;
            set
            {
                _entity = value;
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

        public ObservableCollection<ConnectionModel> Connections
        {
            get
            {
                return _connections;
            }
            set
            {
                _connections = value;
                OnPropertyChanged(nameof(Connections));
            }
        }

        public ObservableCollection<IProcessor> Processors
        {
            get
            {
                return _processors;
            }
            set
            {
                _processors = value;
                OnPropertyChanged(nameof(Processors));
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

        public AttributeContentViewModel(
            IEventAggregator eventAggregator,
            AttributeRepository attributeRepository,
            EntityRepository entityRepository,
            ConnectionRepository connectionRepository,
            IEnumerable<IProcessor> processors)
        {
            this.eventAggregator = eventAggregator;
            this.attributeRepository = attributeRepository;
            this.entityRepository = entityRepository;
            this.connectionRepository = connectionRepository;
            Processors = new ObservableCollection<IProcessor>(processors.Where(p => p.Type == ProcessorType.Attribute));
            Connections = new ObservableCollection<ConnectionModel>(connectionRepository.GetAll());
            Entities = new ObservableCollection<EntityModel>(entityRepository.GetAll());
            Commands = new ObservableCollection<string>(new List<string> { "Save", "New", "Delete", "Preview", "Manage" });
            eventAggregator.GetEvent<SelectAttributeEvent>().Subscribe(OnSelectAttribute);
        }

        private void OnSelectAttribute(SelectAttributeEventArgument obj)
        {
            var attr = attributeRepository.GetById(obj.AttributeId);
            _attribute = attr;
            Name = attr.Name;
            Description = attr.Description;
            Enabled = !attr.HasState(EntityState.Disabled);

            SelectedSourceConnection = Connections.FirstOrDefault(c => c.Id == attr.SourceConnectionId);
            SelectedDestinationConnection = Connections.FirstOrDefault(c => c.Id == attr.DestinationConnectionId);

            SelectedSourceProcessor = Processors.FirstOrDefault(p => p.Id == attr.SourceProcessorId);
            SelectedDestinationProcessor = Processors.FirstOrDefault(p => p.Id == attr.DestinationProcessorId);
        }

        public void SetOptions(IEnumerable<OptionItem> pullerOptions)
        {
            PullerOptions = new ObservableCollection<OptionItemViewModel>(pullerOptions.Select(o =>
            {
                var result = new OptionItemViewModel();
                result.SetOption(o);
                return result;
            }));
        }

        public void SetCommands(List<string> commands)
        {
            
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
            if (_attribute == null)
            {
                message = "No item to delete";
                return true;
            }
            try
            {
                attributeRepository.BeginTransaction();
                attributeRepository.DeleteById(_attribute.Id.ToString());
                attributeRepository.UnlinkOptions(_attribute.Id);
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
                entityRepository.RollBack();
                throw;
            }
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
