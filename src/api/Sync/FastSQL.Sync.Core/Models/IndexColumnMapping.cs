using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FastSQL.Sync.Core.Models
{
    public class IndexColumnMapping: INotifyPropertyChanged
    {
        private bool _key;
        private bool _primary;
        private string _dataType;
        private string _mappingName;
        private string _sourceName;

        public string SourceName {
            get => _sourceName;
            set
            {
                _sourceName = value;
                OnPropertyChanged(nameof(SourceName));
            }
        }
        public string MappingName {
            get => _mappingName;
            set
            {
                _mappingName = value;
                OnPropertyChanged(nameof(MappingName));
            }
        }
        public string DataType {
            get => _dataType;
            set
            {
                _dataType = value;
                OnPropertyChanged(nameof(DataType));
            }
        }
        public bool Primary {
            get => _primary;
            set
            {
                _primary = value;
                OnPropertyChanged(nameof(Primary));
            }
        }
        public bool Key {
            get => _key;
            set
            {
                _key = value;
                OnPropertyChanged(nameof(Key));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
