using Castle.Core;
using DateTimeExtensions;
using FastSQL.App.Events;
using FastSQL.App.Interfaces;
using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Reporters;
using FastSQL.Sync.Core.Repositories;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FastSQL.App.UserControls.Reporters
{
    public class UCReporterContentViewModel: BaseViewModel
    {
        private IEnumerable<IReporter> _reporters;
        private readonly IEventAggregator _eventAggregator;
        private readonly MessageDeliveryChannelRepository _messageDeliveryChannelRepository;
        private ObservableCollection<string> _commands;
        private ObservableCollection<OptionItemViewModel> _options;

        private IReporter _selectedReporter;
        private ReporterModel _reporterModel;
        private string _name;
        private string _description;
        private List<MessageDeliveryChannelModel> _channels;
        private MessageDeliveryChannelModel _selectedChannel;
        private ObservableCollection<MessageDeliveryChannelModel> _selectedChannels;
        private readonly ReporterRepository _reporterRepository;

        public string Name
        {
            get => _name; //_connection?.Name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Description
        {
            get => _description;//_connection?.Description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public ObservableCollection<OptionItemViewModel> Options
        {
            get
            {
                return _options;
            }
            set
            {
                _options = value;
                OnPropertyChanged(nameof(Options));
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

        public ObservableCollection<IReporter> Reporters
        {
            get => new ObservableCollection<IReporter>(_reporters);
            set
            {
                _reporters = value?.ToList() ?? new List<IReporter>();
                OnPropertyChanged(nameof(_reporters));
            }
        }

        public BaseCommand AddSelectedChannelCommand => new BaseCommand(o => true, OnAddChannel);
        
        public BaseCommand RemoveSelectedChannelCommand => new BaseCommand(o => true, OnRemoveChannel);
        
        [DoNotWire]
        public IReporter SelectedReporter
        {
            get => _selectedReporter;
            set
            {
                _selectedReporter = value;
                IEnumerable<OptionModel> options = null;
                if (_reporterModel != null)
                {
                    options = _reporterRepository.LoadOptions(_reporterModel.Id.ToString());
                }

                _selectedReporter.SetOptions(options?.Select(o => new OptionItem { Name = o.Key, Value = o.Value }) ?? new List<OptionItem>());
                SetOptions(_selectedReporter.Options);

                OnPropertyChanged(nameof(SelectedReporter));
            }
        }

        public ObservableCollection<MessageDeliveryChannelModel> Channels
        {
            get => new ObservableCollection<MessageDeliveryChannelModel>(_channels);
            set
            {
                _channels = value?.ToList() ?? new List<MessageDeliveryChannelModel>();
                OnPropertyChanged(nameof(_reporters));
            }
        }

        [DoNotWire]
        public MessageDeliveryChannelModel SelectedChannel
        {
            get => _selectedChannel;
            set
            {
                _selectedChannel = value;
                OnPropertyChanged(nameof(SelectedChannel));
            }
        }

        public ObservableCollection<MessageDeliveryChannelModel> SelectedChannels
        {
            get => _selectedChannels ?? (_selectedChannels = new ObservableCollection<MessageDeliveryChannelModel>());
            set
            {
                _selectedChannels = value ?? new ObservableCollection<MessageDeliveryChannelModel>();
                OnPropertyChanged(nameof(SelectedChannels));
            }
        }

        public UCReporterContentViewModel(
            IEnumerable<IReporter> reporters,
            IEventAggregator eventAggregator,
            MessageDeliveryChannelRepository messageDeliveryChannelRepository,
            ReporterRepository reporterRepository)
        {
            _reporters = reporters;
            _eventAggregator = eventAggregator;
            _messageDeliveryChannelRepository = messageDeliveryChannelRepository;
            _reporterRepository = reporterRepository;

            Commands = new ObservableCollection<string>(new List<string> { "Save", "New", "Delete" });
            eventAggregator.GetEvent<SelectReporterEvent>().Subscribe(OnSelectReporter);
            eventAggregator.GetEvent<RefreshChannelListEvent>().Subscribe(OnChannelsRefresh);
            Reporters = new ObservableCollection<IReporter>(_reporters);
            Channels = new ObservableCollection<MessageDeliveryChannelModel>(_messageDeliveryChannelRepository.GetAll());
        }

        private void OnChannelsRefresh(RefreshChannelListEventArgument obj)
        {
            Channels = new ObservableCollection<MessageDeliveryChannelModel>(_messageDeliveryChannelRepository.GetAll());
        }

        private void OnSelectReporter(SelectReporterEventArgument obj)
        {
            _reporterModel = _reporterRepository.GetById(obj.ReporterId.ToString());
            Name = _reporterModel.Name;
            Description = _reporterModel.Description;
            SelectedReporter = Reporters?.FirstOrDefault(p => p.Id == _reporterModel.ReporterId);
            var selectedChannels = _reporterRepository.GetLinkedDeliveryChannels(new List<string> { _reporterModel.Id.ToString() });
            SelectedChannels = new ObservableCollection<MessageDeliveryChannelModel>(Channels.Where(c => selectedChannels.Any(sc => sc.DeliveryChannelId == c.Id && sc.ReporterId == _reporterModel.Id)));
        }

        public void SetOptions(IEnumerable<OptionItem> options)
        {
            Options = new ObservableCollection<OptionItemViewModel>(options.Select(o =>
            {
                var result = new OptionItemViewModel();
                result.SetOption(o);
                return result;
            }));
        }
        
        private bool Save(out string message)
        {
            if (_reporterModel == null)
            {
                message = "No item to save";
                return true;
            }

            try
            {
                _reporterRepository.BeginTransaction();
                var result = _reporterRepository.Update(_reporterModel.Id.ToString(), new
                {
                    Name,
                    Description,
                    ReporterId = SelectedReporter.Id
                });

                SelectedReporter.SetOptions(Options?.Select(o => new OptionItem { Name = o.Name, Value = o.Value }) ?? new List<OptionItem>());

                _reporterRepository.LinkOptions(_reporterModel.Id.ToString(), SelectedReporter.Options); // no need to unlink, they can be reuse
                _reporterRepository.UnlinkDeliveryChannels(_reporterModel.Id.ToString());
                _reporterRepository.LinkDeliveryChannels(_reporterModel.Id.ToString(), SelectedChannels.Select(c => c.Id.ToString()));
                _reporterRepository.Commit();
                _eventAggregator.GetEvent<RefreshReporterListEvent>().Publish(new RefreshReporterListEventArgument
                {
                    SelectedReporterId = _reporterModel.Id
                });
            }
            catch
            {
                _reporterRepository.RollBack();
                throw;
            }

            message = "Success";
            return true;
        }

        private bool New(out string message)
        {
            try
            {
                _reporterRepository.BeginTransaction();
                var result = _reporterRepository.Create(new
                {
                    Name,
                    Description,
                    ReporterId = SelectedReporter.Id,
                    CreatedAt = DateTime.Now.ToUnixTimestamp()
                });

                SelectedReporter.SetOptions(Options?.Select(o => new OptionItem { Name = o.Name, Value = o.Value }) ?? new List<OptionItem>());

                _reporterRepository.LinkOptions(result, SelectedReporter.Options); // no need to unlink, they can be reuse
                _reporterRepository.UnlinkDeliveryChannels(result);
                _reporterRepository.LinkDeliveryChannels(result, SelectedChannels.Select(c => c.Id.ToString()));
                _reporterRepository.Commit();

                message = "Success";
                _eventAggregator.GetEvent<RefreshReporterListEvent>().Publish(new RefreshReporterListEventArgument
                {
                    SelectedReporterId = Guid.Parse(result)
                });
                return true;
            }
            catch
            {
                _reporterRepository.RollBack();
                throw;
            }
        }

        private void OnAddChannel(object obj)
        {
            if (SelectedChannels.Any(c => c.Id == SelectedChannel?.Id))
            {
                return;
            }
            SelectedChannels.Add(SelectedChannel);
        }

        private void OnRemoveChannel(object obj)
        {
            var channel = (MessageDeliveryChannelModel)obj;
            SelectedChannels.Remove(channel);
        }

        private bool Delete(out string message)
        {
            if (_reporterModel == null)
            {
                message = "No item to delete";
                return true;
            }
            try
            {
                _reporterRepository.BeginTransaction();
                _reporterRepository.DeleteById(_reporterModel.Id.ToString());
                _reporterRepository.UnlinkOptions(_reporterModel.Id.ToString());
                _reporterRepository.UnlinkDeliveryChannels(_reporterModel.Id.ToString());
                _reporterRepository.Commit();

                _eventAggregator.GetEvent<RefreshReporterListEvent>().Publish(new RefreshReporterListEventArgument
                {
                    SelectedReporterId = Guid.Empty
                });

                message = "Success";
                return true;
            }
            catch
            {
                _reporterRepository.RollBack();
                throw;
            }
        }
        
        public BaseCommand ApplyCommand => new BaseCommand(o => true, OnApplyCommand);
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
            }
            MessageBox.Show(
                Application.Current.MainWindow,
                message,
                success ? "Success" : "Failed",
                MessageBoxButton.OK,
                success ? MessageBoxImage.Information : MessageBoxImage.Error);

        }

        public void Loaded()
        {

        }
    }
}
