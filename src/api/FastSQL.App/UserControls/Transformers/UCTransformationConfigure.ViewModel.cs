﻿using FastSQL.App.Interfaces;
using FastSQL.App.ViewModels;
using FastSQL.Core;
using FastSQL.Sync.Core;
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

namespace FastSQL.App.UserControls.Transformers
{
    public class UCTransformationConfigureViewModel : BaseViewModel
    {
        private IEnumerable<ITransformer> transformers;
        private ITransformer _selectedTransformer;
        private ObservableCollection<OptionItemViewModel> _options;
        private Guid _entityId;
        private EntityType _entityType;
        private ObservableCollection<TransformationItemViewModel> _transformations;
        private string _columnName;

        public BaseCommand AddTransformerCommand => new BaseCommand(o => true, OnAddTransformer);
        public BaseCommand RemoveTransformerCommand => new BaseCommand(o => true, OnRemoveTransformer);

        private void OnRemoveTransformer(object obj)
        {
            Transformations.Remove((TransformationItemViewModel)obj);
        }

        private void OnAddTransformer(object obj)
        {
            if (SelectedTransfomer == null)
            {
                MessageBox.Show(Application.Current.MainWindow, "Please choose a transformer", "Error");
                return;
            }

            var exists = Transformations.FirstOrDefault(t => t.ColumnName == ColumnName && t.TransformerId == SelectedTransfomer.Id);
            if (exists != null)
            {
                return;
            }

            var newTransformation = new TransformationItemViewModel();
            SelectedTransfomer.SetOptions(Options.Select(o => new OptionItem { Name = o.Name, Value = o.Value }));
            newTransformation.SetTransformation(new ColumnTransformationModel {
                ColumnName = ColumnName,
                TargetEntityId = _entityId,
                TargetEntityType = _entityType,
                TransformerId = SelectedTransfomer.Id
            });
            newTransformation.TransformerName = SelectedTransfomer.Name;
            newTransformation.SetOptions(SelectedTransfomer.Options);
            Transformations.Add(newTransformation);
        }

        public string ColumnName
        {
            get => _columnName;
            set
            {
                _columnName = value;
                OnPropertyChanged(nameof(ColumnName));
            }
        }

        public ITransformer SelectedTransfomer
        {
            get => _selectedTransformer;
            set
            {
                _selectedTransformer = value;
                Options = new ObservableCollection<OptionItemViewModel>(
                    _selectedTransformer?.Options.Select(o =>
                    {
                        var result = new OptionItemViewModel();
                        result.SetOption(o);
                        return result;
                    }) ?? new List<OptionItemViewModel>());
                OnPropertyChanged(nameof(SelectedTransfomer));
            }
        }

        public ObservableCollection<ITransformer> Transformers
        {
            get => new ObservableCollection<ITransformer>(transformers);
            set
            {
                transformers = value.ToList();
                OnPropertyChanged(nameof(Transformers));
            }
        }
        
        public ObservableCollection<OptionItemViewModel> Options
        {
            get => _options ?? (_options = new ObservableCollection<OptionItemViewModel>());
            set
            {
                _options = value;
                OnPropertyChanged(nameof(Options));
            }
        }

        public ObservableCollection<TransformationItemViewModel> Transformations
        {
            get => _transformations ?? (_transformations = new ObservableCollection<TransformationItemViewModel>());
            set
            {
                _transformations = value;
                OnPropertyChanged(nameof(Transformations));
            }
        }

        public UCTransformationConfigureViewModel(IEnumerable<ITransformer> transformers)
        {
            this.transformers = transformers;
        }

        public void SetIndex(IIndexModel entity)
        {
            if (entity == null)
            {
                return;
            }
            
            _entityId = entity.Id;
            _entityType = entity.EntityType;
            using (var transformerRepository = ResolverFactory.Resolve<TransformerRepository>())
            {
                var transformations = transformerRepository.GetTransformations(_entityId, _entityType);
                Transformations = new ObservableCollection<TransformationItemViewModel>(
                    transformations.Select(t =>
                    {
                        var r = new TransformationItemViewModel();
                        var transformer = transformers.FirstOrDefault(f => f.Id == t.TransformerId);
                        transformer.SetOptions(transformerRepository.LoadOptions(_entityId.ToString(), _entityType).Select(o => new OptionItem { Name = o.Key, Value = o.Value }));
                        r.SetTransformation(t);
                        r.TransformerName = transformer.Name;
                        r.SetOptions(transformer.Options);
                        return r;
                    }));
            }
        }

        public IEnumerable<OptionItem> GetTransformationOptions()
        {
            return Transformations.SelectMany(t =>
            {
                var options = t.Options;
                var transformer = transformers.FirstOrDefault(f => f.Id == t.TransformerId);
                transformer.SetOptions(options.Select(o =>
                {
                    var optionItem = o.GetModel();
                    return optionItem;
                }));
                return transformer.Options;
            });
        }
    }
}
