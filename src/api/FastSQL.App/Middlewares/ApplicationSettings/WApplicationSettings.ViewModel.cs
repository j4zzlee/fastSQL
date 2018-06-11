using FastSQL.App.Interfaces;
using FastSQL.App.UserControls;
using FastSQL.Sync.Core.Settings;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Middlewares.ApplicationSettings
{
    public class WApplicationSettingsViewModel: BaseViewModel
    {
        private UCSettingContentViewModel _settingViewModel;

        public UCSettingContentViewModel SettingViewModel
        {
            get => _settingViewModel;
            set
            {
                _settingViewModel = value;
                OnPropertyChanged(nameof(SettingViewModel));
            }
        }

        public WApplicationSettingsViewModel()
        {

        }
    }
}
