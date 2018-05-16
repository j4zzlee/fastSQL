using FastSQL.App.Interfaces;
using FastSQL.App.Managers;
using FastSQL.App.UserControls.OutputView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FastSQL.App
{
    public class MainWindowViewModel : BaseViewModel
    {
        private bool _isInitialized = false;
        private UCOutputViewViewModel _outputViewViewModel;
        private readonly SettingManager settingManager;
        private readonly IEnumerable<IPageManager> pageManagers;

        public UCOutputViewViewModel OutputViewViewModel
        {
            get => _outputViewViewModel;
            set
            {
                _outputViewViewModel = value;
                OnPropertyChanged(nameof(OutputViewViewModel));
            }
        }

        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
            set
            {
                _isInitialized = value;
                OnPropertyChanged(nameof(IsInitialized));
            }
        }

        public BaseCommand OpenPageCommand => new BaseCommand(obj => IsInitialized, OpenPage);
        public BaseCommand OpenSettingsCommand => new BaseCommand(obj => true, OpenPage);
        public BaseCommand OpenHelpCommand => new BaseCommand(o => true, OpenPage);

        public MainWindowViewModel(SettingManager settingManager, IEnumerable<IPageManager> pageManagers)
        {
            this.settingManager = settingManager;
            this.pageManagers = pageManagers;
        }

        public void ValidateSettings()
        {
            var isOk = settingManager.Validate(out string message);
            IsInitialized = isOk;
            if (!isOk)
            {
                // Load Settings
                OpenPage("LI5b8oVnMUqTxSHIgNj6wQ");
            }
        }

        public void OpenPage(object @params)
        {
            var pageId = @params.ToString();
            var page = pageManagers.FirstOrDefault(p => p.Id == pageId);
            page?.Apply();
        }
    }
}
