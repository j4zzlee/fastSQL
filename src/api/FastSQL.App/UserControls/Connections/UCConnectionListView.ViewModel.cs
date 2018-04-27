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
        private readonly UCConnectionsContent uCConnectionsContent;
        private ConnectionModel _selectedConnection;
        private ObservableCollection<ConnectionModel> _connections;

        public BaseCommand SelectItemCommand => new BaseCommand(o => true, o => {
            var id = o.ToString();
            // A selected setting has been changed
            eventAggregator.GetEvent<AddPageEvent>().Publish(new AddPageEventArgument
            {
                PageDefinition = uCConnectionsContent
            });

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
            IEventAggregator eventAggregator,
            UCConnectionsContent uCConnectionsContent)
        {
            this.connectionRepository = connectionRepository;
            this.eventAggregator = eventAggregator;
            this.uCConnectionsContent = uCConnectionsContent;

            Connections = new ObservableCollection<ConnectionModel>(connectionRepository.GetAll());
        }
    }
}
