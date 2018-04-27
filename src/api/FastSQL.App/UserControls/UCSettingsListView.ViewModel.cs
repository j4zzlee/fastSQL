using FastSQL.App.Events;
using FastSQL.App.Interfaces;
using FastSQL.Core.UI.Events;
using FastSQL.Sync.Core.Settings;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FastSQL.App.UserControls
{
    public class UCSettingsListViewViewModel: BaseViewModel
    {
        private ObservableCollection<ISettingProvider> _settings;
        private ISettingProvider _selectedSetting;
        private readonly IEventAggregator eventAggregator;
        private readonly UCSettingsContent uCSettingsContent;

        public ObservableCollection<ISettingProvider> Settings
        {
            get
            {
                return _settings;
            }
            set
            {
                _settings = value ?? new ObservableCollection<ISettingProvider>();
                OnPropertyChanged(nameof(Settings));
            }
        }

        public ISettingProvider SelectedSettiing
        {
            get { return _selectedSetting; }
            set
            {
                _selectedSetting = value;

                

                OnPropertyChanged(nameof(SelectedSettiing));
            }
        }

        public BaseCommand SelectItemCommand => new BaseCommand(o => true, o => {
            var id = o.ToString();
            // A selected setting has been changed
            eventAggregator.GetEvent<AddPageEvent>().Publish(new AddPageEventArgument
            {
                PageDefinition = uCSettingsContent
            });

            eventAggregator.GetEvent<SelectSettingEvent>().Publish(new SelectSettingEventArgument
            {
                SettingId = id
            });
        });

        public UCSettingsListViewViewModel (
            IEnumerable<ISettingProvider> settingManagers,
            IEventAggregator eventAggregator,
            UCSettingsContent uCSettingsContent)
        {
            Settings = new ObservableCollection<ISettingProvider>(settingManagers);
            this.eventAggregator = eventAggregator;
            this.uCSettingsContent = uCSettingsContent;
        }
    }
}
