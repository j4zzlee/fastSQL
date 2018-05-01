using FastSQL.App.Events;
using FastSQL.App.Interfaces;
using FastSQL.App.ViewModels;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using Newtonsoft.Json.Linq;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FastSQL.App.UserControls.Dependencies
{
    public class AttributeDependencyViewModel: BaseViewModel
    {
        private ObservableCollection<string> _dependOnSteps;
        private ObservableCollection<string> _stepsToExecute;
        private ObservableCollection<DependencyItemViewModel> _dependencies;
        private ObservableCollection<AttributeModel> _attributes;
        private readonly EntityRepository entityRepository;
        private readonly AttributeRepository attributeRepository;

        private AttributeModel _selectedTargetAttribute;
        private object _entity;

        private string _dependOnStep;
        private string _stepToExecute;
        private bool _executeImmediately;

        public ObservableCollection<string> DependOnSteps
        {
            get => _dependOnSteps;
            set
            {
                _dependOnSteps = value;
                OnPropertyChanged(nameof(DependOnSteps));
            }
        }

        public ObservableCollection<string> StepsToExecute
        {
            get => _stepsToExecute;
            set
            {
                _stepsToExecute = value;
                OnPropertyChanged(nameof(StepsToExecute));
            }
        }

        public string SelectedDependOnStep
        {
            get => _dependOnStep;
            set
            {
                _dependOnStep = value;
                OnPropertyChanged(nameof(SelectedDependOnStep));
            }
        }

        public string SelectedStepToExecute
        {
            get => _stepToExecute;
            set
            {
                _stepToExecute = value;
                OnPropertyChanged(nameof(SelectedStepToExecute));
            }
        }

        public bool ExecuteImmediately
        {
            get => _executeImmediately;
            set
            {
                _executeImmediately = value;
                OnPropertyChanged(nameof(ExecuteImmediately));
            }
        }

        public ObservableCollection<DependencyItemViewModel> Dependencies
        {
            get => _dependencies;
            set
            {
                _dependencies = value;
                OnPropertyChanged(nameof(Dependencies));
            }
        }

        public ObservableCollection<AttributeModel> TargetAttributes
        {
            get => _attributes;
            set
            {
                _attributes = value;
                OnPropertyChanged(nameof(TargetAttributes));
            }
        }

        public AttributeModel SelectedTargetAttribute
        {
            get => _selectedTargetAttribute;
            set
            {
                _selectedTargetAttribute = value;
                OnPropertyChanged(nameof(SelectedTargetAttribute));
            }
        }

        public BaseCommand AddDependencyCommand => new BaseCommand(o => true, OnAddDependency);
        public BaseCommand RemoveDependencyCommand => new BaseCommand(o => true, OnRemoveDependency);

        private void OnAddDependency(object obj)
        {
            if (SelectedTargetAttribute == null)
            {
                MessageBox.Show("Missing target dependency.");
                return;
            }
            JObject entity = null;

            if (_entity != null)
            {
                entity = JObject.FromObject(_entity);
            }

            var dependOnStep = string.IsNullOrWhiteSpace(SelectedDependOnStep) ? IntegrationStep.Push : (IntegrationStep)Enum.Parse(typeof(IntegrationStep), SelectedDependOnStep);
            var stepToExecute = string.IsNullOrWhiteSpace(SelectedStepToExecute) ? IntegrationStep.Push : (IntegrationStep)Enum.Parse(typeof(IntegrationStep), SelectedStepToExecute);
            var exists = Dependencies.FirstOrDefault(d => d.TargetEntityId == SelectedTargetAttribute.Id
                && d.TargetEntityType == SelectedTargetAttribute.EntityType
                && d.DependOnStep == dependOnStep
                && d.StepToExecute == stepToExecute);
            if (exists != null)
            {
                return;
            }

            Dependencies.Add(new DependencyItemViewModel
            {
                EntityId = _entity == null ? Guid.NewGuid() : Guid.Parse(entity.GetValue("Id").ToString()),
                EntityType = _entity == null ? EntityType.Entity : (EntityType)Enum.Parse(typeof(EntityType), entity.GetValue("EntityType").ToString()),
                DependOnStep = string.IsNullOrWhiteSpace(SelectedDependOnStep) ? IntegrationStep.Push : (IntegrationStep)Enum.Parse(typeof(IntegrationStep), SelectedDependOnStep),
                StepToExecute = string.IsNullOrWhiteSpace(SelectedStepToExecute) ? IntegrationStep.Push : (IntegrationStep)Enum.Parse(typeof(IntegrationStep), SelectedStepToExecute),
                ExecuteImmediately = ExecuteImmediately,
                TargetEntityId = SelectedTargetAttribute.Id,
                TargetEntityType = SelectedTargetAttribute.EntityType,
                DependOn = SelectedTargetAttribute.Name
            });
        }

        private void OnRemoveDependency(object obj)
        {
            Dependencies.Remove((DependencyItemViewModel)obj);
        }

        public AttributeDependencyViewModel(
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            IEventAggregator eventAggregator)
        {
            this.entityRepository = entityRepository;
            this.attributeRepository = attributeRepository;
            DependOnSteps = new ObservableCollection<string>(Enum.GetNames(typeof(IntegrationStep)));
            StepsToExecute = new ObservableCollection<string>(Enum.GetNames(typeof(IntegrationStep)));
            TargetAttributes = new ObservableCollection<AttributeModel>(attributeRepository.GetAll());
            Dependencies = new ObservableCollection<DependencyItemViewModel>();
            eventAggregator.GetEvent<RefreshAttributeListEvent>().Subscribe(OnRefreshAttribute);
        }

        private void OnRefreshAttribute(RefreshAttributeListEventArgument obj)
        {
            TargetAttributes = new ObservableCollection<AttributeModel>(attributeRepository.GetAll());
        }

        public void SetEntity(object entity)
        {
            _entity = entity;
            if (entity != null)
            {
                var jEntity = JObject.FromObject(entity);
                var entityId = Guid.Parse(jEntity.GetValue("Id").ToString());
                var entityType = (EntityType)Enum.Parse(typeof(EntityType), jEntity.GetValue("EntityType").ToString());
                IEnumerable<DependencyItemModel> dependencies = new List<DependencyItemModel>();
                switch (entityType)
                {
                    case EntityType.Entity:
                        dependencies = entityRepository.GetDependencies(entityId, EntityType.Attribute);
                        break;
                    case EntityType.Attribute:
                        dependencies = attributeRepository.GetDependencies(entityId, EntityType.Attribute);
                        break;
                }

                Dependencies = new ObservableCollection<DependencyItemViewModel>(
                            dependencies
                            .Select(d => new DependencyItemViewModel
                             {
                                 Id = d.Id,
                                 DependOn = TargetAttributes.FirstOrDefault(e => e.Id == d.TargetEntityId)?.Name,
                                 DependOnStep = d.DependOnStep,
                                 EntityId = entityId,
                                 EntityType = entityType,
                                 ExecuteImmediately = d.ExecuteImmediately,
                                 StepToExecute = d.StepToExecute,
                                 TargetEntityId = d.TargetEntityId,
                                 TargetEntityType = d.TargetEntityType
                             }));
            }
        }
    }
}
