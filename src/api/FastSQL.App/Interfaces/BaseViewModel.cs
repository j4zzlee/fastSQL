using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Interfaces
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        protected object Owner;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetOwner(object owner)
        {
            Owner = owner;
        }
    }
}
