using FastSQL.App.Interfaces;
using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.ViewModels
{
    public class ProviderViewModel : BaseViewModel
    {
        private IRichProvider _provider;

        public string Name
        {
            get => _provider?.Name;
            set { }
        }

        public string DisplayName
        {
            get => _provider?.DisplayName;
            set { }
        }

        public string Id
        {
            get => _provider?.Id;
            set { }
        }

        public string Description
        {
            get => _provider?.Description;
            set { }
        }
        
        public ProviderViewModel()
        {

        }

        public void SetPrivider(IRichProvider provider)
        {
            _provider = provider;
            Name = provider.Name;
            DisplayName = provider.DisplayName;
            Description = provider.Description;
            Id = provider.Id;
        }
    }
}
