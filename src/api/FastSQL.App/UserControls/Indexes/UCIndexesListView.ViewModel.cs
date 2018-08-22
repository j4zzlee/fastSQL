using FastSQL.App.Events;
using FastSQL.App.Interfaces;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.UserControls.Indexes
{
    public class UCIndexesListViewViewModel : BaseViewModel
    {
        private readonly EntityRepository entityRepository;
        private readonly AttributeRepository attributeRepository;
        private readonly IEventAggregator eventAggregator;
        private IIndexModel _selectedIndexModel;
        private ObservableCollection<IIndexModel> _indexModels;
        private EntityType _indexType;
        private int _selectedIndex;

        public BaseCommand SelectItemCommand => new BaseCommand(o => true, o =>
        {
            eventAggregator.GetEvent<SelectIndexEvent>().Publish(new SelectIndexEventArgument
            {
                IndexModel = IndexModels.FirstOrDefault(i => i.Id == Guid.Parse(o.ToString()))
            });
        });

        public ObservableCollection<IIndexModel> IndexModels
        {
            get
            {
                return _indexModels;
            }
            set
            {
                _indexModels = value ?? new ObservableCollection<IIndexModel>();
                OnPropertyChanged(nameof(IndexModels));
            }
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }

        public IIndexModel SelectedIndexModel
        {
            get { return _selectedIndexModel; }
            set
            {
                _selectedIndexModel = value;

                if (value == null)
                {
                    SelectedIndex = 0;
                }
                else
                {
                    SelectedIndex = IndexModels?
                    .Select((i, idx) => new { i, idx })
                    .FirstOrDefault(i => i.i?.Id == value?.Id)?.idx ?? 0;
                }
                
                OnPropertyChanged(nameof(SelectedIndexModel));
            }
        }

        public UCIndexesListViewViewModel(
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            IEventAggregator eventAggregator)
        {
            this.entityRepository = entityRepository;
            this.attributeRepository = attributeRepository;
            this.eventAggregator = eventAggregator;
            eventAggregator.GetEvent<RefreshIndexesListViewEvent>().Subscribe(OnRefreshIndexes);
        }

        public void SetIndexType(EntityType indexType)
        {
            _indexType = indexType;
            LoadIndexModels();
        }

        private void LoadIndexModels()
        {
            if (_indexType == EntityType.Entity)
            {
                IndexModels = new ObservableCollection<IIndexModel>(entityRepository.GetAll());
            }
            else
            {
                IndexModels = new ObservableCollection<IIndexModel>(attributeRepository.GetAll());
            }
        }


        public void Loaded()
        {

        }

        private void OnRefreshIndexes(RefreshIndexesListViewEventArgument obj)
        {
            if (obj.SelectedIndexType != _indexType)
            {
                return;
            }
            LoadIndexModels();
            var selectedId = obj.SelectedIndexId;
            IIndexModel first = IndexModels.FirstOrDefault(f => f.Id.ToString() == obj.SelectedIndexId);
            if (string.IsNullOrWhiteSpace(obj.SelectedIndexId))
            {
                first = IndexModels.FirstOrDefault();
                selectedId = first?.Id.ToString();
            }
            if (!string.IsNullOrWhiteSpace(selectedId))
            {
                eventAggregator.GetEvent<SelectIndexEvent>().Publish(new SelectIndexEventArgument
                {
                    IndexModel = first
                });
            }
        }
    }
}
