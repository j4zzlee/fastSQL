using FastSQL.App.Interfaces;
using FastSQL.App.ViewModels;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FastSQL.App.UserControls
{
    public class EntityDependencyViewModel: BaseViewModel
    {
        private ObservableCollection<string> _dependOnSteps;
        private ObservableCollection<string> _stepsToExecute;
        private ObservableCollection<DependencyItemViewModel> _dependencies;
        private ObservableCollection<EntityModel> _entities;

        private readonly EntityRepository entityRepository;

        private EntityModel _selectedTargetEntity;
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

        public ObservableCollection<EntityModel> TargetEntities
        {
            get => _entities;
            set
            {
                _entities = value;
                OnPropertyChanged(nameof(Entities));
            }
        }

        public EntityModel SelectedTargetEntity
        {
            get => _selectedTargetEntity;
            set
            {
                _selectedTargetEntity = value;
                OnPropertyChanged(nameof(SelectedTargetEntity));
            }
        }

        public BaseCommand AddDependencyCommand => new BaseCommand(o => true, OnAddDependency);
        public BaseCommand RemoveDependencyCommand => new BaseCommand(o => true, OnRemoveDependency);

        private void OnAddDependency(object obj)
        {
            if (SelectedTargetEntity == null)
            {
                MessageBox.Show("Missing target dependency.");
                return;
            }
            JObject entity = null;

            if (_entity != null)
            {
                entity = JObject.FromObject(_entity);
            }

            Dependencies.Add(new DependencyItemViewModel
            {
                EntityId = _entity == null ? Guid.NewGuid() : Guid.Parse(entity.GetValue("Id").ToString()),
                EntityType = _entity == null ? EntityType.Entity : (EntityType)Enum.Parse(typeof(EntityType), entity.GetValue("EntityType").ToString()),
                DependOnStep = string.IsNullOrWhiteSpace(SelectedDependOnStep) ? IntegrationStep.Push : (IntegrationStep) Enum.Parse(typeof(IntegrationStep), SelectedDependOnStep),
                StepToExecute = string.IsNullOrWhiteSpace(SelectedStepToExecute) ? IntegrationStep.Push : (IntegrationStep)Enum.Parse(typeof(IntegrationStep), SelectedStepToExecute),
                ExecuteImmediately = ExecuteImmediately,
                TargetEntityId = SelectedTargetEntity.Id,
                TargetEntityType = SelectedTargetEntity.EntityType,
                DependOn = SelectedTargetEntity.Name
            });
        }

        private void OnRemoveDependency(object obj)
        {
            Dependencies.Remove((DependencyItemViewModel)obj);
        }

        public EntityDependencyViewModel(EntityRepository entityRepository)
        {
            this.entityRepository = entityRepository;
            DependOnSteps = new ObservableCollection<string>(Enum.GetNames(typeof(IntegrationStep)));
            StepsToExecute = new ObservableCollection<string>(Enum.GetNames(typeof(IntegrationStep)));
            TargetEntities = new ObservableCollection<EntityModel>(entityRepository.GetAll());
            Dependencies = new ObservableCollection<DependencyItemViewModel>();
        }

        public void SetEntity(object entity)
        {
            _entity = entity;
            //Dependencies = new ObservableCollection<DependencyItemViewModel> {
            //    new DependencyItemViewModel
            //    {
            //        Id = Guid.NewGuid(),
            //        DependOn = "Product",
            //    }
            //};
        }
    }
}
