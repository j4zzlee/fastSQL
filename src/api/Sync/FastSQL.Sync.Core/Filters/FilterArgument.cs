using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FastSQL.Sync.Core.Filters
{
    public enum FilterType
    {
        None = 0, // normal
        Expression = 1
    }
    public class FilterArgument
    {
        public string Field { get; set; }
        public string @Op { get; set; }
        public string Target { get; set; }
        public FilterType FilterType { get; set; }
    }

    public class ObservableFilterArgument: INotifyPropertyChanged
    {
        private string _field;
        private string _op;
        private string _target;
        private FilterArgument _arg;

        public string Field {
            get => _field;
            set
            {
                _field = value;
                OnPropertyChanged(nameof(Field));
            }
        }
        public string @Op {
            get => _op;
            set
            {
                _op = value;
                OnPropertyChanged(nameof(@Op));
            }
        }
        public string Target {
            get => _target;
            set
            {
                _target = value;
                OnPropertyChanged(nameof(Target));
            }
        }

        public ObservableFilterArgument From(FilterArgument arg)
        {
            _arg = arg;
            Field = arg.Field;
            Op = arg.Op;
            Target = arg.Target;
            return this;
        }

        public FilterArgument ToFilterArgument()
        {
            return new FilterArgument
            {
                Field = Field,
                Op = Op,
                Target = Target
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class FilterArguments
    {
        public IEnumerable<FilterArgument> Filters { get; set; }
        public IDictionary<string, string> Orders { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
    }
}
