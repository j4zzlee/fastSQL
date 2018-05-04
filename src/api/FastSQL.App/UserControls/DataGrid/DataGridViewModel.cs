using FastSQL.App.Interfaces;
using FastSQL.Sync.Core.Filters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.UserControls.DataGrid
{
    public class DataGridViewModel : BaseViewModel
    {
        private int _totalCount;
        private int _pageNumber;
        private int _limit;
        private int _offset;
        private string _fieldFilter;
        private string _opFilter;
        private string _targetFilter;
        private bool _userFilter;
        private bool _usePaging;
        private int _totalPages;
        private bool _hasFirstButton;
        private bool _hasPreviousButton;
        private bool _hasNextButton;
        private bool _hasLastButton;
        private bool _hasPreviousText;
        private bool _hasNextText;
        private bool _hasPaging;

        private ObservableCollection<object> _data;
        private ObservableCollection<ObservableFilterArgument> _filters;
        private ObservableCollection<string> _filterProperties;
        private ObservableCollection<string> _gridContextMenus;
        private bool _hasGridContextMenu;
        private object _selectedItem;

        public delegate void FilterDelegate(object sender, FilterArguments args);

        public event FilterDelegate OnFilter;

        public BaseCommand AddFilterCommand => new BaseCommand(o => true, OnAddFilter);
        public BaseCommand RemoveFilterCommand => new BaseCommand(o => true, OnRemoveFilter);
        public BaseCommand GoToFirstPageCommand => new BaseCommand(o => true, OnGoToFirstPage);
        public BaseCommand GoToPreviousPageCommand => new BaseCommand(o => true, OnGoToPreviousPage);
        public BaseCommand GoToNextPageCommand => new BaseCommand(o => true, OnGoToNextPage);
        public BaseCommand GoToLastPageCommand => new BaseCommand(o => true, OnGoToLastPage);
        public BaseCommand OnGridContextMenuCommand => new BaseCommand(o => true, OnGridContextMenuSelected);

        public bool HasFirstButton
        {
            get => _hasFirstButton;
            set
            {
                _hasFirstButton = value;
                OnPropertyChanged(nameof(HasFirstButton));
            }
        }

        public bool HasPreviousButton
        {
            get => _hasPreviousButton;
            set
            {
                _hasPreviousButton = value;
                OnPropertyChanged(nameof(HasPreviousButton));
            }
        }

        public bool HasNextButton
        {
            get => _hasNextButton;
            set
            {
                _hasNextButton = value;
                OnPropertyChanged(nameof(HasNextButton));
            }
        }

        public bool HasLastButton
        {
            get => _hasLastButton;
            set
            {
                _hasLastButton = value;
                OnPropertyChanged(nameof(HasLastButton));
            }
        }

        public bool HasPreviousText
        {
            get => _hasPreviousText;
            set
            {
                _hasPreviousText = value;
                OnPropertyChanged(nameof(HasPreviousText));
            }
        }

        public bool HasNextText
        {
            get => _hasNextText;
            set
            {
                _hasNextText = value;
                OnPropertyChanged(nameof(HasNextText));
            }
        }

        public string FieldFilter
        {
            get => _fieldFilter;
            set
            {
                _fieldFilter = value;
                OnPropertyChanged(nameof(FieldFilter));
            }
        }

        public string OpFilter
        {
            get => _opFilter;
            set
            {
                _opFilter = value;
                OnPropertyChanged(nameof(OpFilter));
            }
        }

        public string TargetFilter
        {
            get => _targetFilter;
            set
            {
                _targetFilter = value;
                OnPropertyChanged(nameof(TargetFilter));
            }
        }

        public int PageNumber
        {
            get => _pageNumber;
            set
            {
                _pageNumber = value;
                OnPropertyChanged(nameof(PageNumber));
                OnPropertyChanged(nameof(HasPaging));
            }
        }

        public int TotalCount
        {
            get => _totalCount;
            set
            {
                _totalCount = value;
                OnPropertyChanged(nameof(TotalCount));
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set
            {
                _totalPages = value;
                OnPropertyChanged(nameof(TotalPages));
            }
        }

        public bool UseFilter
        {
            get => _userFilter;
            set
            {
                _userFilter = value;
                OnPropertyChanged(nameof(UseFilter));
            }
        }

        public bool UsePaging
        {
            get => _usePaging;
            set
            {
                _usePaging = value;
                OnPropertyChanged(nameof(UsePaging));
                OnPropertyChanged(nameof(HasPaging));
            }
        }

        public bool HasPaging
        {
            get => _hasPaging || (UsePaging && TotalPages > 1);
            set
            {
                _hasPaging = value;
                OnPropertyChanged(nameof(HasPaging));
            }
        }

        public bool HasGridContextMenu
        {
            get => _hasGridContextMenu || GridContextMenus?.Count > 0;
            set
            {
                _hasGridContextMenu = value;
                OnPropertyChanged(nameof(HasGridContextMenu));
            }
        }

        public object SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public ObservableCollection<string> GridContextMenus
        {
            get => _gridContextMenus;
            set
            {
                _gridContextMenus = value;
                OnPropertyChanged(nameof(GridContextMenus));
                OnPropertyChanged(nameof(HasGridContextMenu));
            }
        }
        
        public ObservableCollection<object> Data
        {
            get => _data ?? (_data = new ObservableCollection<object>());
            set
            {
                _data = value;
                OnPropertyChanged(nameof(Data));
            }
        }

        public ObservableCollection<string> FilterProperties
        {
            get => _filterProperties ?? (_filterProperties = new ObservableCollection<string>());
            set
            {
                _filterProperties = value ?? new ObservableCollection<string>();
                OnPropertyChanged(nameof(FilterProperties));
            }
        }

        public ObservableCollection<ObservableFilterArgument> Filters
        {
            get => _filters ?? (_filters = new ObservableCollection<ObservableFilterArgument>());
            set
            {
                _filters = value;
                OnPropertyChanged(nameof(Filters));
            }
        }

        public ObservableCollection<string> FilterOperators
        {
            get => new ObservableCollection<string> { "=", ">", "<", ">=", "<=", "Like" };
        }

        private void OnRemoveFilter(object obj)
        {
            Filters.Remove((ObservableFilterArgument)obj);
            Filter(Filters.Select(f => f.ToFilterArgument()));
        }

        private void OnAddFilter(object obj)
        {
            if (string.IsNullOrWhiteSpace(FieldFilter) || string.IsNullOrWhiteSpace(OpFilter) || string.IsNullOrWhiteSpace(TargetFilter))
            {
                return;
            }
            var exists = Filters.Any(f => f.Field == FieldFilter && f.Op == OpFilter && f.Target == TargetFilter);
            if (exists)
            {
                return;
            }

            Filters.Add(new ObservableFilterArgument
            {
                Field = FieldFilter,
                Op = OpFilter,
                Target = TargetFilter
            });
            Filter(Filters.Select(f => f.ToFilterArgument()));
        }

        public DataGridViewModel()
        {
            CalculatePage();
            ShowPagingControls();
        }

        public void SetData(IEnumerable<object> data)
        {
            Data = new ObservableCollection<object>(data);
            if (data?.Count() > 0)
            {
                var first = data.First();
                var jFirst = JObject.FromObject(first);
                var props = jFirst.Properties();
                FilterProperties = new ObservableCollection<string>(props.Select(p => p.Name));
            }
        }

        public void SetOffset(int limit, int offset)
        {
            _limit = limit;
            _offset = offset;
            CalculatePage();
            ShowPagingControls();
            //Offset = offset;
        }

        public void SetTotalCount(int totalCount)
        {
            TotalCount = totalCount;
            CalculatePage();
            ShowPagingControls();
        }

        public void SetGridContextMenus(List<string> list)
        {
            GridContextMenus = new ObservableCollection<string>(list);
        }

        private void ShowPagingControls()
        {
            if (TotalCount == 0 || TotalPages == 1)
            {
                //UsePaging = false;
                HasPaging = false;
                return;
            }

            HasPaging = true;

            if (PageNumber <= 1)
            {
                HasFirstButton = false;
                HasPreviousButton = false;
                HasPreviousText = false;
            }

            if (PageNumber >= TotalPages)
            {
                HasLastButton = false;
                HasNextButton = false;
                HasNextText = false;
            }
        }

        private void CalculatePage()
        {
            if (_limit == 0 || TotalCount == 0)
            {
                PageNumber = 0;
                TotalPages = 0;
                return;
            }
            if (TotalCount <= _limit)
            {
                PageNumber = 0;
                TotalPages = 1;
                return;
            }
            var pages = TotalCount / _limit;
            var left = TotalCount % _limit;
            TotalPages = left > 0 ? pages + 1 : pages;
            var pageOffset = _offset / _limit;
            var pageOffsetLeft = _offset % _limit;
            if (_offset < _limit)
            {
                PageNumber = 1;
                return;
            }
            PageNumber = pageOffsetLeft > 0 ? pageOffset + 1 : pageOffset;
        }

        private void OnGoToFirstPage(object obj)
        {
            throw new NotImplementedException();
        }
        private void OnGoToPreviousPage(object obj)
        {
            throw new NotImplementedException();
        }
        private void OnGoToNextPage(object obj)
        {
            throw new NotImplementedException();
        }
        private void OnGoToLastPage(object obj)
        {
            throw new NotImplementedException();
        }
        private void OnGridContextMenuSelected(object obj)
        {
            throw new NotImplementedException();
        }

        public void Filter(IEnumerable<FilterArgument> args)
        {
            OnFilter?.Invoke(this, new FilterArguments { Filters = args });
        }
    }
}
