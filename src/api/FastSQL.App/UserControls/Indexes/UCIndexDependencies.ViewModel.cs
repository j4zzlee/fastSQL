using FastSQL.App.Events;
using FastSQL.App.Interfaces;
using FastSQL.App.ViewModels;
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
using System.Windows;

namespace FastSQL.App.UserControls.Indexes
{
    public class UCIndexDependenciesViewModel : BaseViewModel
    {
        private EntityType _targetIndexType;
        private EntityType _dependencyIndexType;
        private IIndexModel _indexModel;

        private ObservableCollection<string> _dependOnSteps;
        private ObservableCollection<string> _stepsToExecute;
        private ObservableCollection<DependencyItemViewModel> _dependencies;
        private ObservableCollection<IIndexModel> _indexModels;

        private readonly EntityRepository entityRepository;
        private readonly AttributeRepository attributeRepository;
        private IIndexModel _selectedIndexModel;

        private string _dependOnStep;
        private string _stepToExecute;
        private bool _executeImmediately;
        private string _foreignKeys;
        private string _referenceKeys;
        private string _indexName;

        public string IndexName
        {
            get => _indexName;
            set
            {
                _indexName = value;
                OnPropertyChanged(nameof(IndexName));
            }
        }

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

        public string ForeignKeys
        {
            get => _foreignKeys;
            set
            {
                _foreignKeys = value;
                OnPropertyChanged(nameof(ForeignKeys));
            }
        }

        public string ReferenceKeys
        {
            get => _referenceKeys;
            set
            {
                _referenceKeys = value;
                OnPropertyChanged(nameof(ReferenceKeys));
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

        public ObservableCollection<IIndexModel> IndexModels
        {
            get => _indexModels;
            set
            {
                _indexModels = value;
                OnPropertyChanged(nameof(IndexModels));
            }
        }

        public IIndexModel SelectedIndexModel
        {
            get => _selectedIndexModel;
            set
            {
                _selectedIndexModel = value;
                OnPropertyChanged(nameof(SelectedIndexModel));
            }
        }

        public BaseCommand AddDependencyCommand => new BaseCommand(o => true, OnAddDependency);
        public BaseCommand RemoveDependencyCommand => new BaseCommand(o => true, OnRemoveDependency);

        private void OnAddDependency(object obj)
        {
            if (SelectedIndexModel == null)
            {
                MessageBox.Show("Missing target dependency.");
                return;
            }

            if (string.IsNullOrWhiteSpace(ForeignKeys) || string.IsNullOrWhiteSpace(ReferenceKeys))
            {
                MessageBox.Show("Missing Foreign Keys or Reference Keys");
                return;
            }
            
            var dependOnStep = string.IsNullOrWhiteSpace(SelectedDependOnStep) ? IntegrationStep.Push : (IntegrationStep)Enum.Parse(typeof(IntegrationStep), SelectedDependOnStep);
            var stepToExecute = string.IsNullOrWhiteSpace(SelectedStepToExecute) ? IntegrationStep.Push : (IntegrationStep)Enum.Parse(typeof(IntegrationStep), SelectedStepToExecute);
            var exists = Dependencies.FirstOrDefault(d => d.TargetEntityId == SelectedIndexModel.Id
                && d.TargetEntityType == SelectedIndexModel.EntityType
                && d.DependOnStep == dependOnStep
                && d.StepToExecute == stepToExecute);
            if (exists != null)
            {
                return;
            }

            Dependencies.Add(new DependencyItemViewModel
            {
                EntityId = _indexModel.Id,
                EntityType = _indexModel.EntityType,
                DependOnStep = dependOnStep,
                StepToExecute = stepToExecute,
                ExecuteImmediately = ExecuteImmediately,
                TargetEntityId = SelectedIndexModel.Id,
                TargetEntityType = SelectedIndexModel.EntityType,
                DependOn = SelectedIndexModel.Name,
                ForeignKeys = ForeignKeys,
                ReferenceKeys = ReferenceKeys
            });
        }

        private void OnRemoveDependency(object obj)
        {
            Dependencies.Remove((DependencyItemViewModel)obj);
        }

        public UCIndexDependenciesViewModel(
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            IEventAggregator eventAggregator)
        {
            this.entityRepository = entityRepository;
            this.attributeRepository = attributeRepository;
            DependOnSteps = new ObservableCollection<string>(Enum.GetNames(typeof(IntegrationStep)));
            StepsToExecute = new ObservableCollection<string>(Enum.GetNames(typeof(IntegrationStep)));
            Dependencies = new ObservableCollection<DependencyItemViewModel>();
            eventAggregator.GetEvent<RefreshIndexesListViewEvent>().Subscribe(OnRefrehsIndexes);
        }

        private void OnRefrehsIndexes(RefreshIndexesListViewEventArgument obj)
        {
            LoadIndexModels();
        }
        
        public void SetTargetType(EntityType indexType)
        {
            _targetIndexType = indexType;
            IndexName = indexType == EntityType.Entity ? "Entity Dependencies" : "Attribute Dependencies";
        }

        public void SetDependencyType(EntityType indexType)
        {
            _dependencyIndexType = indexType;
            LoadIndexModels();
        }

        private void LoadIndexModels()
        {
            switch (_dependencyIndexType)
            {
                case EntityType.Entity:
                    IndexModels = new ObservableCollection<IIndexModel>(entityRepository.GetAll());
                    break;
                case EntityType.Attribute:
                    IndexModels = new ObservableCollection<IIndexModel>(attributeRepository.GetAll());
                    break;
                default: return;
            }
        }

        public void SetIndex(IIndexModel indexModel)
        {
            _indexModel = indexModel;
            
            if (_indexModel == null)
            {
                return;
            }
            
            var entityId = _indexModel.Id;
            var entityType = _indexModel.EntityType;
            var dependencies = entityRepository.GetDependencies(entityId, entityType, _dependencyIndexType);

            Dependencies = new ObservableCollection<DependencyItemViewModel>(
                        dependencies
                        .Select(d => new DependencyItemViewModel
                        {
                            Id = d.Id,
                            DependOn = IndexModels.FirstOrDefault(e => e.Id == d.TargetEntityId)?.Name,
                            DependOnStep = d.DependOnStep,
                            EntityId = entityId,
                            EntityType = entityType,
                            ExecuteImmediately = d.ExecuteImmediately,
                            StepToExecute = d.StepToExecute,
                            TargetEntityId = d.TargetEntityId,
                            TargetEntityType = d.TargetEntityType,
                            ForeignKeys = d.ForeignKeys,
                            ReferenceKeys = d.ReferenceKeys
                        }));
        }
    }
}
