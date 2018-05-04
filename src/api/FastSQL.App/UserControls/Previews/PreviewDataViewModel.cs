using FastSQL.App.Interfaces;
using FastSQL.App.UserControls.DataGrid;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.UserControls.Previews
{
    public class PreviewDataViewModel : BaseViewModel
    {
        private IPuller _puller;
        private SyncState _status;
        private EntityModel _entity;
        private AttributeModel _attribute;
        private DataGridViewModel _dataGridViewModel;
        
        public SyncState Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(HasData));
            }
        }

        public bool HasData
        {
            get => _status.HasFlag(SyncState.HasData);
            set
            {
                _status = value ? _status | SyncState.HasData : (_status | SyncState.HasData) ^ SyncState.HasData;
                OnPropertyChanged(nameof(HasData));
            }
        }

        public DataGridViewModel DataGridViewModel {
            get => _dataGridViewModel;
            set
            {
                _dataGridViewModel = value;
                OnPropertyChanged(nameof(DataGridViewModel));
            }
        }

        public PreviewDataViewModel(DataGridViewModel dataGridViewModel)
        {
            DataGridViewModel = dataGridViewModel;
        }

        public void SetPuller(IPuller puller)
        {
            _puller = puller;
        }

        public void SetEntity(EntityModel model)
        {
            _entity = model;
        }

        public void SetAttribute(AttributeModel model)
        {
            _attribute = model;
        }

        public void Load()
        {
            var res = _puller.Preview();
            DataGridViewModel.SetData(res.Data);
            DataGridViewModel.SetGridContextMenus(new List<string> { "Change" });
            Status = res.Status;
        }
    }
}
