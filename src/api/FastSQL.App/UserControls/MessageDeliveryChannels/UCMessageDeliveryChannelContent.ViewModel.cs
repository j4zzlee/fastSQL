using Castle.Core;
using DateTimeExtensions;
using FastSQL.App.Events;
using FastSQL.App.Interfaces;
using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.MessageDeliveryChannels;
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

namespace FastSQL.App.UserControls.MessageDeliveryChannels
{
    public class UCMessageDeliveryChannelContentViewModel : BaseViewModel
    {
        private IEnumerable<IMessageDeliveryChannel> _channels;
        private readonly IEventAggregator _eventAggregator;
        private ObservableCollection<string> _commands;
        private ObservableCollection<OptionItemViewModel> _options;

        private IMessageDeliveryChannel _selectedChannel;
        private MessageDeliveryChannelModel _channelModel;
        private string _name;
        private string _description;
        private readonly MessageDeliveryChannelRepository _messageDeliveryChannelRepository;

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

        public ObservableCollection<IMessageDeliveryChannel> Channels
        {
            get => new ObservableCollection<IMessageDeliveryChannel>(_channels);
            set
            {
                _channels = value?.ToList() ?? new List<IMessageDeliveryChannel>();
                OnPropertyChanged(nameof(_channels));
            }
        }

        [DoNotWire]
        public IMessageDeliveryChannel SelectedChannel
        {
            get => _selectedChannel;
            set
            {
                _selectedChannel = value;
                IEnumerable<OptionModel> options = null;
                if (_selectedChannel != null)
                {
                    options = _messageDeliveryChannelRepository.LoadOptions(_channelModel.Id.ToString());
                }

                _selectedChannel.SetOptions(options?.Select(o => new OptionItem { Name = o.Key, Value = o.Value }) ?? new List<OptionItem>());
                SetOptions(_selectedChannel.Options);

                OnPropertyChanged(nameof(SelectedChannel));
            }
        }

        public UCMessageDeliveryChannelContentViewModel(
            IEnumerable<IMessageDeliveryChannel> channels,
            IEventAggregator eventAggregator,
            MessageDeliveryChannelRepository messageDeliveryChannelRepository)
        {
            _channels = channels;
            _eventAggregator = eventAggregator;
            _messageDeliveryChannelRepository = messageDeliveryChannelRepository;

            Commands = new ObservableCollection<string>(new List<string> { "Try Connect", "Save", "New", "Delete" });
            eventAggregator.GetEvent<SelectChannelEvent>().Subscribe(OnSelectChannel);
            Channels = new ObservableCollection<IMessageDeliveryChannel>(_channels);
        }

        private void OnSelectChannel(SelectChannelEventArgument obj)
        {
            _channelModel = _messageDeliveryChannelRepository.GetById(obj.ChannelId.ToString());
            Name = _channelModel.Name;
            Description = _channelModel.Description;
            SelectedChannel = Channels?.FirstOrDefault(p => p.Id == _channelModel.ChannelId);
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

        private async Task<bool> TryConnect()
        {
            SelectedChannel.SetOptions(Options?.Select(o => new OptionItem { Name = o.Name, Value = o.Value }) ?? new List<OptionItem>());
            await SelectedChannel.DeliverMessage("----HexaSync Bot Test Information----", MessageType.Information);
            await SelectedChannel.DeliverMessage("----HexaSync Bot Test Error----", MessageType.Error);
            await SelectedChannel.DeliverMessage("----HexaSync Bot Test Exception----", MessageType.Exception);
            return true;
        }

        private bool Save(out string message)
        {
            if (_channelModel == null)
            {
                message = "No item to save";
                return true;
            }

            try
            {
                _messageDeliveryChannelRepository.BeginTransaction();
                var result = _messageDeliveryChannelRepository.Update(_channelModel.Id.ToString(), new
                {
                    Name,
                    Description,
                    ChannelId = SelectedChannel.Id
                });

                SelectedChannel.SetOptions(Options?.Select(o => new OptionItem { Name = o.Name, Value = o.Value }) ?? new List<OptionItem>());

                _messageDeliveryChannelRepository.LinkOptions(_channelModel.Id.ToString(), SelectedChannel.Options);
                _messageDeliveryChannelRepository.Commit();
                _eventAggregator.GetEvent<RefreshChannelListEvent>().Publish(new RefreshChannelListEventArgument
                {
                    SelectedChannelId = _channelModel.Id
                });
            }
            catch
            {
                _messageDeliveryChannelRepository.RollBack();
                throw;
            }

            message = "Success";
            return true;
        }

        private bool New(out string message)
        {
            try
            {
                _messageDeliveryChannelRepository.BeginTransaction();
                var result = _messageDeliveryChannelRepository.Create(new
                {
                    Name,
                    Description,
                    ChannelId = SelectedChannel.Id,
                    CreatedAt = DateTime.Now.ToUnixTimestamp()
                });

                SelectedChannel.SetOptions(Options?.Select(o => new OptionItem { Name = o.Name, Value = o.Value }) ?? new List<OptionItem>());

                _messageDeliveryChannelRepository.LinkOptions(result, SelectedChannel.Options);
                _messageDeliveryChannelRepository.Commit();

                message = "Success";
                _eventAggregator.GetEvent<RefreshChannelListEvent>().Publish(new RefreshChannelListEventArgument
                {
                    SelectedChannelId = Guid.Parse(result)
                });
                return true;
            }
            catch
            {
                _messageDeliveryChannelRepository.RollBack();
                throw;
            }
        }

        private bool Delete(out string message)
        {
            if (_channelModel == null)
            {
                message = "No item to delete";
                return true;
            }
            try
            {
                _messageDeliveryChannelRepository.BeginTransaction();
                _messageDeliveryChannelRepository.DeleteById(_channelModel.Id.ToString());
                _messageDeliveryChannelRepository.UnlinkOptions(_channelModel.Id.ToString());
                _messageDeliveryChannelRepository.Commit();

                _eventAggregator.GetEvent<RefreshChannelListEvent>().Publish(new RefreshChannelListEventArgument
                {
                    SelectedChannelId = Guid.Empty
                });

                message = "Success";
                return true;
            }
            catch
            {
                _messageDeliveryChannelRepository.RollBack();
                throw;
            }
        }
        
        public BaseCommand ApplyCommand => new BaseCommand(o => true, OnApplyCommand);
        private async void OnApplyCommand(object obj)
        {
            var commandText = obj.ToString();
            var message = "Command not available";
            var success = false;
            switch (commandText)
            {
                case "Try Connect":
                    success = await Task.Run(async () => await TryConnect());
                    message = "Message has sent";
                    break;
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
