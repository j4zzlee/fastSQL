﻿using FastSQL.Core;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Interfaces
{
    public class BaseViewModel : INotifyPropertyChanged, IDisposable
    {
        protected object Owner;

        public RepositoryFactory RepositoryFactory { get; set; }
        public ResolverFactory ResolverFactory { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void Dispose()
        {
            RepositoryFactory.Release(this);
        }

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
