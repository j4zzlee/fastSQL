using FastSQL.App.Events;
using FastSQL.App.Interfaces;
using FastSQL.Core.UI.Events;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.UserControls.Connections
{
    public class UCConnectionListViewViewModel: BaseViewModel
    {
        private readonly ConnectionRepository connectionRepository;
        private readonly IEventAggregator eventAggregator;
        private ConnectionModel _selectedConnection;
        private ObservableCollection<ConnectionModel> _connections;

        public BaseCommand SelectItemCommand => new BaseCommand(o => true, o => {
            var id = o.ToString();
            eventAggregator.GetEvent<SelectConnectionEvent>().Publish(new SelectConnectionEventArgument
            {
                ConnectionId = id
            });
        });

        public ObservableCollection<ConnectionModel> Connections
        {
            get
            {
                return _connections;
            }
            set
            {
                _connections = value ?? new ObservableCollection<ConnectionModel>();
                OnPropertyChanged(nameof(Connections));
            }
        }

        public ConnectionModel SelectedConnection
        {
            get { return _selectedConnection; }
            set
            {
                _selectedConnection = value;
                
                OnPropertyChanged(nameof(SelectedConnection));
            }
        }

        public UCConnectionListViewViewModel(
            ConnectionRepository connectionRepository,
            IEventAggregator eventAggregator)
        {
            this.connectionRepository = connectionRepository;
            this.eventAggregator = eventAggregator;
            Connections = new ObservableCollection<ConnectionModel>(connectionRepository.GetAll());
            eventAggregator.GetEvent<RefreshConnectionListEvent>().Subscribe(OnRefreshConnections);
            var firstConection = Connections.FirstOrDefault();
            if (firstConection != null)
            {
                eventAggregator.GetEvent<SelectConnectionEvent>().Publish(new SelectConnectionEventArgument
                {
                    ConnectionId = firstConection.Id.ToString()
                });
            }
        }

        private void OnRefreshConnections(RefreshConnectionListEventArgument obj)
        {
            Connections = new ObservableCollection<ConnectionModel>(connectionRepository.GetAll());
            var selectedId = obj.SelectedConnectionId;
            if (string.IsNullOrWhiteSpace(obj.SelectedConnectionId))
            {
                var firstConnection = Connections.FirstOrDefault();
                selectedId = firstConnection?.Id.ToString();
            }
            if (!string.IsNullOrWhiteSpace(selectedId))
            {
                eventAggregator.GetEvent<SelectConnectionEvent>().Publish(new SelectConnectionEventArgument
                {
                    ConnectionId = selectedId
                });
            }
        }
    }
}
