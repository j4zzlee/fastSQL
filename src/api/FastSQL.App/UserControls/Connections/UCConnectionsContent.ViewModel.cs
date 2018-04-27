using FastSQL.App.Interfaces;
using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FastSQL.App.UserControls.Connections
{
    public class UCConnectionsContentViewModel : BaseViewModel
    {
        private readonly IEnumerable<IRichAdapter> adapters;
        private readonly IEnumerable<IRichProvider> providers;
        private ConnectionModel _connection;
        private ObservableCollection<string> _commands;
        private ObservableCollection<OptionItemViewModel> _options;

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

        public ObservableCollection<string> Commands {
            get => _commands;
            set
            {
                _commands = value ?? new ObservableCollection<string>();
                OnPropertyChanged(nameof(Commands));
            }
        }

        public UCConnectionsContentViewModel(IEnumerable<IRichAdapter> adapters, IEnumerable<IRichProvider> providers)
        {
            this.adapters = adapters;
            this.providers = providers;
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

        public void SetCommands(List<string> commands)
        {
            Commands = new ObservableCollection<string>(commands);
        }

        private bool TryConnect(out string message)
        {
            var adapter = adapters.FirstOrDefault(p => p.IsProvider(_connection.ProviderId));
            adapter.SetOptions(Options.Select(o => new OptionItem { Name = o.Name, Value = o.Value }));
            return adapter.TryConnect(out message);
        }

        public void SetConnection(ConnectionModel connection)
        {
            _connection = connection;
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
                    success = await Task.Run(() => TryConnect(out message));
                    break;
                case "Save": break;
                case "Delete": break;
            }
            MessageBox.Show(message,
                success ? "Success" : "Failed",
                MessageBoxButton.OK,
                success ? MessageBoxImage.Information : MessageBoxImage.Error);
            //_settingProvider.SetOptions(Options.ToList().Select(o => new OptionItem
            //{
            //    Name = o.Name,
            //    Value = o.Value
            //}));

            //var success = await Task.Run(() => _settingProvider.Invoke(commandText, out message));

        }
    }
}
