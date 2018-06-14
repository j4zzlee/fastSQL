using FastSQL.App.Events;
using FastSQL.App.Interfaces;
using FastSQL.Core.UI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.UserControls.OptionItems
{
    public class UCGridOptionsViewModel<T> : BaseViewModel
        where T : INotifyPropertyChanged
    {
        private T _selectedItem;
        private Action<string> _onValueUpdated;
        private IEnumerable<T> _selectedItems;
        public delegate void DataGridEventDelegate(object sender, DataGridCommandEventArgument args);
        public event DataGridEventDelegate OnEvent;
        public BaseCommand SelectionChangedCommand => new BaseCommand(o => true, OnSelectedItems);
        public BaseCommand OnGridContextMenuCommand => new BaseCommand(o => true, OnGridContextMenuSelected);

        private ItemsChangeObservableCollection<T> _data;

        public ItemsChangeObservableCollection<T> Data
        {
            get => _data;
            set
            {
                _data = value;
                OnPropertyChanged(nameof(Data));
            }
        }
        
        public UCGridOptionsViewModel()
        {
            PropertyChanged += UCGridOptionsViewModel_PropertyChanged;
        }

        private void UCGridOptionsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == nameof(Data))
            //{

            //}
        }

        private void OnGridContextMenuSelected(object obj)
        {
            switch (obj.ToString())
            {
                case "Remove":
                    if (_selectedItems == null)
                    {
                        break;
                    }
                    for (var i = _selectedItems.Count() - 1; i >= 0; i--)
                    {
                        Data.Remove(_selectedItems.ElementAt(i));
                    }
                    break;
            }
            OnEvent?.Invoke(this, new DataGridCommandEventArgument
            {
                CommandName = obj as string,
                SelectedItems = _selectedItems?.Cast<object>() ?? new List<object>()
            });
        }

        private void OnSelectedItems(object obj)
        {
            _selectedItems = (obj as System.Collections.IList).Cast<T>();
        }

        public void SetData(object gridData)
        {
            // There is no other way
            var dataProp = this.GetType().GetProperty("Data");
            var dataType = typeof(ItemsChangeObservableCollection<T>);
            var constructedGridData = Activator.CreateInstance(dataType, gridData);
            dataProp.SetValue(this, constructedGridData);
            Data.CollectionChanged += Data_CollectionChanged;
        }

        private void Data_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //var t = _context.GetType();
            //var sourceTypeProp = t.GetProperty("SourceType");
            //var sourceType = sourceTypeProp.GetValue(_context, null) as Type;
            //var valueUpdatedMethod = _viewModel.GetType().GetMethod("ValueUpdated");
            ////_viewModel = Activator.CreateInstance(castedViewModelType);
            //valueUpdatedMethod.Invoke(_viewModel, null);
            ValueUpdated();
            //throw new NotImplementedException();
        }

        public void OnValueUpdated(Action<string> onValueUpdated)
        {
            _onValueUpdated = onValueUpdated;
        }

        public void ValueUpdated()
        {
            _onValueUpdated?.Invoke(JsonConvert.SerializeObject(Data));
        }
    }
}
