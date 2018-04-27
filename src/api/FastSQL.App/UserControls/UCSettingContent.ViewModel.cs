using FastSQL.App.Interfaces;
using FastSQL.Core;
using FastSQL.Sync.Core.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FastSQL.App.UserControls
{
    public class UCSettingContentViewModel : BaseViewModel
    {
        private ObservableCollection<OptionItemViewModel> _options;
        private ObservableCollection<string> _commands;
        private ISettingProvider _settingProvider;

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
            get
            {
                return _commands;
            }
            set
            {
                _commands = value;
                OnPropertyChanged(nameof(Commands));
            }
        }


        public BaseCommand ApplyCommand => new BaseCommand(o => true, OnApplyCommand);
        private async void OnApplyCommand(object obj)
        {
            var commandText = obj.ToString();
            _settingProvider.SetOptions(Options.ToList().Select(o => new OptionItem {
                Name = o.Name,
                Value = o.Value
            }));
            var message = string.Empty;
            var success = await Task.Run(() => _settingProvider.Invoke(commandText, out message));
            MessageBox.Show(message,
                success ? "Success" : "Failed",
                MessageBoxButton.OK,
                success ? MessageBoxImage.Information : MessageBoxImage.Error);
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
        
        public void SetCommands(IEnumerable<string> commands)
        {
            Commands = new ObservableCollection<string>(commands);
        }

        public void SetProvider(ISettingProvider settingProvider)
        {
            _settingProvider = settingProvider;
        }
    }
}
