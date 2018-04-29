using FastSQL.App.Events;
using FastSQL.App.Interfaces;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.UserControls.Attributes
{
    public class AttributesListViewViewModel: BaseViewModel
    {
        private readonly AttributeRepository entityRepository;
        private readonly IEventAggregator eventAggregator;
        private AttributeModel _selectedAttribute;
        private ObservableCollection<AttributeModel> _attributes;

        public BaseCommand SelectItemCommand => new BaseCommand(o => true, o => {
            var id = o.ToString();
            eventAggregator.GetEvent<SelectAttributeEvent>().Publish(new SelectAttributeEventArgument
            {
                AttributeId = id
            });
        });

        public ObservableCollection<AttributeModel> Attributes
        {
            get
            {
                return _attributes;
            }
            set
            {
                _attributes = value ?? new ObservableCollection<AttributeModel>();
                OnPropertyChanged(nameof(Attributes));
            }
        }

        public AttributeModel SelectedAttribute
        {
            get { return _selectedAttribute; }
            set
            {
                _selectedAttribute = value;

                OnPropertyChanged(nameof(SelectedAttribute));
            }
        }

        public AttributesListViewViewModel(
            AttributeRepository entityRepository,
            IEventAggregator eventAggregator)
        {
            this.entityRepository = entityRepository;
            this.eventAggregator = eventAggregator;
            Attributes = new ObservableCollection<AttributeModel>(entityRepository.GetAll());
            eventAggregator.GetEvent<RefreshAttributeListEvent>().Subscribe(OnRefreshAttributes);
            var first = Attributes.FirstOrDefault();
            if (first != null)
            {
                eventAggregator.GetEvent<SelectAttributeEvent>().Publish(new SelectAttributeEventArgument
                {
                    AttributeId = first.Id.ToString()
                });
            }
        }

        private void OnRefreshAttributes(RefreshAttributeListEventArgument obj)
        {
            Attributes = new ObservableCollection<AttributeModel>(entityRepository.GetAll());
            var selectedId = obj.SelectedAttributeId;
            if (string.IsNullOrWhiteSpace(obj.SelectedAttributeId))
            {
                var firstAttribute = Attributes.FirstOrDefault();
                selectedId = firstAttribute?.Id.ToString();
            }
            if (!string.IsNullOrWhiteSpace(selectedId))
            {
                eventAggregator.GetEvent<SelectAttributeEvent>().Publish(new SelectAttributeEventArgument
                {
                    AttributeId = selectedId
                });
            }
        }
    }
}
