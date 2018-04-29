using FastSQL.App.Events;
using FastSQL.App.Interfaces;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Linq;

namespace FastSQL.App.UserControls.Entities
{
    public class EntitiesListViewViewModel: BaseViewModel
    {
        private readonly EntityRepository entityRepository;
        private readonly IEventAggregator eventAggregator;
        private EntityModel _selectedEntity;
        private ObservableCollection<EntityModel> _entities;

        public BaseCommand SelectItemCommand => new BaseCommand(o => true, o => {
            var id = o.ToString();
            eventAggregator.GetEvent<SelectEntityEvent>().Publish(new SelectEntityEventArgument
            {
                EntityId = id
            });
        });

        public ObservableCollection<EntityModel> Entities
        {
            get
            {
                return _entities;
            }
            set
            {
                _entities = value ?? new ObservableCollection<EntityModel>();
                OnPropertyChanged(nameof(Entities));
            }
        }

        public EntityModel SelectedEntity
        {
            get { return _selectedEntity; }
            set
            {
                _selectedEntity = value;

                OnPropertyChanged(nameof(SelectedEntity));
            }
        }

        public EntitiesListViewViewModel(
            EntityRepository entityRepository,
            IEventAggregator eventAggregator)
        {
            this.entityRepository = entityRepository;
            this.eventAggregator = eventAggregator;
            Entities = new ObservableCollection<EntityModel>(entityRepository.GetAll());
            eventAggregator.GetEvent<RefreshEntityListEvent>().Subscribe(OnRefreshEntities);
            var first = Entities.FirstOrDefault();
            if (first != null)
            {
                eventAggregator.GetEvent<SelectEntityEvent>().Publish(new SelectEntityEventArgument
                {
                    EntityId = first.Id.ToString()
                });
            }
        }

        private void OnRefreshEntities(RefreshEntityListEventArgument obj)
        {
            Entities = new ObservableCollection<EntityModel>(entityRepository.GetAll());
            var selectedId = obj.SelectedEntityId;
            if (string.IsNullOrWhiteSpace(obj.SelectedEntityId))
            {
                var firstEntity = Entities.FirstOrDefault();
                selectedId = firstEntity?.Id.ToString();
            }
            if (!string.IsNullOrWhiteSpace(selectedId))
            {
                eventAggregator.GetEvent<SelectEntityEvent>().Publish(new SelectEntityEventArgument
                {
                    EntityId = selectedId
                });
            }
        }
    }
}
