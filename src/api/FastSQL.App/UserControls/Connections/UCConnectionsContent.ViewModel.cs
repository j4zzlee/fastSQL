using FastSQL.App.Events;
using FastSQL.App.Interfaces;
using FastSQL.App.ViewModels;
using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using Prism.Events;
using st2forget.utils.sql;
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
        private IEnumerable<IRichProvider> providers;
        private readonly IEventAggregator eventAggregator;
        private readonly ConnectionRepository connectionRepository;
        private readonly ResolverFactory resolverFactory;
        private ConnectionModel _connection;
        private ObservableCollection<string> _commands;
        private ObservableCollection<OptionItemViewModel> _options;
        private IRichProvider _selectedProvider;

        public string Name
        {
            get => _connection?.Name;
            set
            {
                if (_connection != null)
                {
                    _connection.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Description
        {
            get => _connection?.Description;
            set
            {
                if (_connection != null)
                {
                    _connection.Description = value;
                    OnPropertyChanged(nameof(Description));
                }
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

        public ObservableCollection<IRichProvider> Providers
        {
            get => new ObservableCollection<IRichProvider>(providers);
            set
            {
                providers = value?.ToList() ?? new List<IRichProvider>();
                OnPropertyChanged(nameof(Providers));
            }
        }

        public IRichProvider SelectedProvider
        {
            get => _selectedProvider;
            set
            {
                _selectedProvider = value;
                IEnumerable<OptionModel> options = null;
                if (_connection != null)
                {
                    options = connectionRepository.LoadOptions(_connection.Id);
                }

                _selectedProvider.SetOptions(options?.Select(o => new OptionItem { Name = o.Key, Value = o.Value }) ?? new List<OptionItem>());
                SetOptions(_selectedProvider.Options);

                OnPropertyChanged(nameof(SelectedProvider));
            }
        }

        public UCConnectionsContentViewModel(
            IEnumerable<IRichAdapter> adapters,
            IEnumerable<IRichProvider> providers,
            IEventAggregator eventAggregator,
            ConnectionRepository connectionRepository,
            ResolverFactory resolverFactory)
        {
            this.adapters = adapters;
            this.providers = providers;
            this.eventAggregator = eventAggregator;
            this.connectionRepository = connectionRepository;
            this.resolverFactory = resolverFactory;
            eventAggregator.GetEvent<SelectConnectionEvent>().Subscribe(OnSelectConnection);
            Providers = new ObservableCollection<IRichProvider>(providers);
        }

        private void OnSelectConnection(SelectConnectionEventArgument obj)
        {
            var connection = connectionRepository.GetById(obj.ConnectionId);

            SetConnection(connection);

            SelectedProvider = Providers?.FirstOrDefault(p => p.Id == connection.ProviderId);

            SetCommands(new List<string> { "Try Connect", "Save", "New", "Delete" });
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
            var adapter = adapters.FirstOrDefault(p => p.IsProvider(SelectedProvider?.Id));
            adapter.SetOptions(SelectedProvider.Options);
            return adapter.TryConnect(out message);
        }

        private bool Save(out string message)
        {
            if (_connection == null)
            {
                message = "No item to save";
                return true;
            }

            try
            {
                connectionRepository.BeginTransaction();
                var result = connectionRepository.Update(_connection.Id.ToString(), new
                {
                    Name,
                    Description,
                    ProviderId = SelectedProvider.Id
                });

                SelectedProvider.SetOptions(Options?.Select(o => new OptionItem { Name = o.Name, Value = o.Value }) ?? new List<OptionItem>());

                connectionRepository.LinkOptions(_connection.Id, SelectedProvider.Options);
                connectionRepository.Commit();
            }
            catch
            {
                connectionRepository.RollBack();
                throw;
            }
            
            message = "Success";
            return true;
        }

        private bool New(out string message)
        {
            try
            {
                connectionRepository.BeginTransaction();
                var result = connectionRepository.Create(new
                {
                    Name,
                    Description,
                    ProviderId = SelectedProvider.Id
                });

                SelectedProvider.SetOptions(Options?.Select(o => new OptionItem { Name = o.Name, Value = o.Value }) ?? new List<OptionItem>());

                connectionRepository.LinkOptions(Guid.Parse(result), SelectedProvider.Options);
                connectionRepository.Commit();

                message = "Success";
                eventAggregator.GetEvent<RefreshConnectionListEvent>().Publish(new RefreshConnectionListEventArgument
                {
                    SelectedConnectionId = result
                });
                return true;
            }
            catch
            {
                connectionRepository.RollBack();
                throw;
            }
        }

        private bool Delete(out string message)
        {
            if (_connection == null)
            {
                message = "No item to delete";
                return true;
            }
            try
            {
                connectionRepository.BeginTransaction();
                connectionRepository.DeleteById(_connection.Id.ToString());
                connectionRepository.UnlinkOptions(_connection.Id);
                connectionRepository.Commit();

                eventAggregator.GetEvent<RefreshConnectionListEvent>().Publish(new RefreshConnectionListEventArgument
                {
                    SelectedConnectionId = string.Empty
                });
                
                message = "Success";
                return true;
            }
            catch
            {
                connectionRepository.RollBack();
                throw;
            }
        }

        public void SetConnection(ConnectionModel connection)
        {
            _connection = connection;
            Name = connection.Name;
            Description = connection.Description;
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
    }
}
