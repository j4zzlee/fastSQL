using FastSQL.App.Interfaces;
using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.UserControls
{
    public class OptionItemViewModel : BaseViewModel
    {
        private OptionItem item;

        public void SetOption(OptionItem item)
        {
            this.item = item;
            this.Name = item.Name;
            this.DisplayName = item.DisplayName;
            this.Example = item.Example;
            this.Description = item.Description;
            this.Value = item.Value;
            this.Source = item.Source;
            this.SourceType = item.SourceType;
            this.OptionGroupNames = item.OptionGroupNames;
        }

        public OptionItem GetModel()
        {
            return new OptionItem
            {
                Name = Name,
                Value = Value
            };
        }

        public string Name
        {
            get => item.Name;
            set
            {
                item.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string DisplayName
        {
            get => item.DisplayName;
            set
            {
                item.DisplayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }
        public string Example
        {
            get => item.Example;
            set
            {
                item.Example = value;
                OnPropertyChanged(nameof(Example));
            }
        }
        public string Description
        {
            get => item.Description;
            set
            {
                item.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        public string Value
        {
            get => item.Value;
            set
            {
                item.Value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public OptionType OptionType
        {
            get => item.Type;
            set
            {
                item.Type = value;
                OnPropertyChanged(nameof(OptionType));
            }
        }

        public OptionItemSource Source
        {
            get => item.Source;
            set
            {
                item.Source = value;
                OnPropertyChanged(nameof(Source));
            }
        }

        public Type SourceType
        {
            get => item.SourceType;
            set
            {
                item.SourceType = value;
                OnPropertyChanged(nameof(SourceType));
            }
        }

        public List<string> OptionGroupNames { get; private set; }
    }
}
