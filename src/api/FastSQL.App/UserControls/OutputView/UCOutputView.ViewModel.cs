using FastSQL.App.Interfaces;
using FastSQL.Core.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FastSQL.App.UserControls.OutputView
{
    public class UCOutputViewViewModel: BaseViewModel
    {
        private bool _hasChannels;
        private string _selectedChannel;
        private ObservableCollection<string> _messages;
        private ObservableCollection<string> _channels;
        private Dictionary<string, List<string>> _channelMessages;
        
        public bool HasChannels
        {
            get => _hasChannels;
            set
            {
                _hasChannels = value;
                OnPropertyChanged(nameof(HasChannels));
            }
        }

        public ObservableCollection<string> Channels
        {
            get => _channels;
            set
            {
                _channels = value;
                OnPropertyChanged(nameof(Channels));
            }
        }

        public string SelectedChannel
        {
            get => _selectedChannel;
            set
            {
                _selectedChannel = value;
                Messages = _channelMessages.ContainsKey(value) && _channelMessages[value].Count > 0 
                    ? new ObservableCollection<string>(_channelMessages[value])
                    : new ObservableCollection<string>();
                OnPropertyChanged(nameof(SelectedChannel));
            }
        }
        
        public ObservableCollection<string> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        public UCOutputViewViewModel(IEventAggregator eventAggregator)
        {
            Messages = new ObservableCollection<string>();
            eventAggregator.GetEvent<ApplicationOutputEvent>().Subscribe(OnApplicationOutput);
            _channelMessages = new Dictionary<string, List<string>>();
            Channels = new ObservableCollection<string>();
        }

        private void OnApplicationOutput(ApplicationOutputEventArgument obj)
        {
            if (!HasChannels || string.IsNullOrWhiteSpace(obj.Channel))
            {
                App.Current.Dispatcher.Invoke(delegate
                {
                    if (Messages.Count >= 500)
                    {
                        Messages.RemoveAt(0);
                    }
                    Messages.Add(obj.Message);
                });
                return;
            }
            
            App.Current.Dispatcher.Invoke(delegate
            {
                if (!_channelMessages.ContainsKey(obj.Channel))
                {
                    _channelMessages.Add(obj.Channel, new List<string>());
                    Channels.Add(obj.Channel);
                }

                if (obj.Channel != SelectedChannel)
                {
                    SelectedChannel = obj.Channel;
                }

                if (Messages.Count >= 500)
                {
                    Messages.RemoveAt(0);
                    _channelMessages[obj.Channel].RemoveAt(0);
                }
                Messages.Add(obj.Message);
                _channelMessages[obj.Channel].Add(obj.Message);
            });
          
        }
    }
}
