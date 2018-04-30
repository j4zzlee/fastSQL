using FastSQL.App.Interfaces;
using FastSQL.App.UserControls;
using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.ViewModels
{
    public class TransformationItemViewModel : BaseViewModel
    {
        private Guid _id;
        private string _columnName;
        private Guid _targetEntityId;
        private EntityType _targetEntityType;
        private string _transformerId;
        private EntityType _entityType;
        private ObservableCollection<OptionItemViewModel> _options;
        public string TransformerName { get; set; }

        public TransformationItemViewModel()
        {
            _options = new ObservableCollection<OptionItemViewModel>();
        }

        public Guid Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
        public string ColumnName
        {
            get => _columnName;
            set
            {
                _columnName = value;
                OnPropertyChanged(nameof(Id));
            }
        }
        public Guid TargetEntityId
        {
            get => _targetEntityId;
            set
            {
                _targetEntityId = value;
                OnPropertyChanged(nameof(Id));
            }
        }
        public EntityType TargetEntityType
        {
            get => _targetEntityType;
            set
            {
                _targetEntityType = value;
                OnPropertyChanged(nameof(Id));
            }
        }
        public string TransformerId
        {
            get => _transformerId;
            set
            {
                _transformerId = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public EntityType EntityType
        {
            get => _entityType;
            set
            {
                _entityType = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public ObservableCollection<OptionItemViewModel> Options
        {
            get => _options;
            set
            {
                _options = value;
                OnPropertyChanged(nameof(Options));
            }
        }

        public void SetTransformation(ColumnTransformationModel model)
        {
            Id = model.Id;
            ColumnName = model.ColumnName;
            TargetEntityId = model.TargetEntityId;
            TargetEntityType = model.TargetEntityType;
            TransformerId = model.TransformerId;
            EntityType = model.EntityType;
        }

        public ColumnTransformationModel GetModel()
        {
            return new ColumnTransformationModel
            {
                Id = Id,
                ColumnName = ColumnName,
                TargetEntityId = TargetEntityId,
                TargetEntityType = TargetEntityType,
                TransformerId = TransformerId
            };
        }

        public void SetOptions(IEnumerable<OptionItem> options)
        {
            Options = new ObservableCollection<OptionItemViewModel>(
                options.Select(o =>
                {
                    var r = new OptionItemViewModel();
                    r.SetOption(o);
                    return r;
                }));
        }
    }
}
