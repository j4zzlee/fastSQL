using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FastSQL.Sync.Core.Models
{
    public class CSVColumnMapping: INotifyPropertyChanged
    {
        private string _sourceName;
        private string _csvHeader;

        public string SourceName {
            get => _sourceName;
            set
            {
                _sourceName = value;
                OnPropertyChanged(nameof(SourceName));
            }
        }
        public string CsvHeader {
            get => _csvHeader;
            set
            {
                _csvHeader = value;
                OnPropertyChanged(nameof(CsvHeader));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
