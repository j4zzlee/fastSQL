using FastSQL.App.Events;
using FastSQL.App.Interfaces;
using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using Prism.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FastSQL.App.UserControls.Connections
{
    public class UCConnectionsContentViewModel : BaseViewModel
    {
        private readonly IEnumerable<IRichAdapter> adapters;
        private IEnumerable<IRichProvider> providers;
        private readonly IEventAggregator eventAggregator;
        private ObservableCollection<string> _commands;
        private ObservableCollection<OptionItemViewModel> _options;
        private IRichProvider _selectedProvider;

        private ConnectionModel _connection;
        private string _name;
        private string _description;

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
                using (var connectionRepository = ResolverFactory.Resolve<ConnectionRepository>())
                {
                    _selectedProvider = value;
                    IEnumerable<OptionModel> options = null;
                    if (_connection != null)
                    {
                        options = connectionRepository.LoadOptions(_connection.Id.ToString());
                    }

                    _selectedProvider.SetOptions(options?.Select(o => new OptionItem { Name = o.Key, Value = o.Value }) ?? new List<OptionItem>());
                    SetOptions(_selectedProvider.Options);

                    OnPropertyChanged(nameof(SelectedProvider));
                }
            }
        }

        public UCConnectionsContentViewModel(
            IEnumerable<IRichAdapter> adapters,
            IEnumerable<IRichProvider> providers,
            IEventAggregator eventAggregator)
        {
            this.adapters = adapters;
            this.providers = providers;
            this.eventAggregator = eventAggregator;

            Commands = new ObservableCollection<string>(new List<string> { "Try Connect", "Save", "New", "Delete" });
            eventAggregator.GetEvent<SelectConnectionEvent>().Subscribe(OnSelectConnection);
            Providers = new ObservableCollection<IRichProvider>(providers);
        }

        private void OnSelectConnection(SelectConnectionEventArgument obj)
        {
            using (var connectionRepository = ResolverFactory.Resolve<ConnectionRepository>())
            {
                var connection = connectionRepository.GetById(obj.ConnectionId);
                Name = connection.Name;
                Description = connection.Description;
                SetConnection(connection);

                SelectedProvider = Providers?.FirstOrDefault(p => p.Id == connection.ProviderId);
            }
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
            var connectionRepository = ResolverFactory.Resolve<ConnectionRepository>();

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

                connectionRepository.LinkOptions(_connection.Id.ToString(), SelectedProvider.Options);
                connectionRepository.Commit();
                eventAggregator.GetEvent<RefreshConnectionListEvent>().Publish(new RefreshConnectionListEventArgument
                {
                    SelectedConnectionId = _connection.Id.ToString()
                });
            }
            catch
            {
                connectionRepository.RollBack();
                throw;
            }
            finally
            {
                connectionRepository?.Dispose();
            }

            message = "Success";
            return true;
        }

        private bool New(out string message)
        {
            var connectionRepository = ResolverFactory.Resolve<ConnectionRepository>();

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

                connectionRepository.LinkOptions(result, SelectedProvider.Options);
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
            finally
            {
                connectionRepository?.Dispose();
            }
        }

        private bool Delete(out string message)
        {
            if (_connection == null)
            {
                message = "No item to delete";
                return true;
            }
            var connectionRepository = ResolverFactory.Resolve<ConnectionRepository>();

            try
            {
                connectionRepository.BeginTransaction();
                connectionRepository.DeleteById(_connection.Id.ToString());
                connectionRepository.UnlinkOptions(_connection.Id.ToString());
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
            finally
            {
                connectionRepository?.Dispose();
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
