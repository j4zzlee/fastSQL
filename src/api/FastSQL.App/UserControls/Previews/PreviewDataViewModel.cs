﻿using FastSQL.App.Interfaces;
using FastSQL.App.UserControls.DataGrid;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Puller;
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
        private IIndexModel _index;
        private IPuller _puller;
        private PullState _status;
        private DataGridViewModel _dataGridViewModel;
        
        public PullState Status
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
            get => _status.HasFlag(PullState.HasData);
            set
            {
                _status = value ? _status | PullState.HasData : (_status | PullState.HasData) ^ PullState.HasData;
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

        public PreviewDataViewModel()
        {
        }

        public void SetPuller(IPuller puller)
        {
            _puller = puller;
        }
        
        internal void SetIndex(IIndexModel model)
        {
            _index = model;
        }

        public Task<int> Load()
        {
            var res = _puller.Preview();
            DataGridViewModel.SetData(res.Data);
            Status = res.Status;
            return Task.FromResult(0);
        }
    }
}
