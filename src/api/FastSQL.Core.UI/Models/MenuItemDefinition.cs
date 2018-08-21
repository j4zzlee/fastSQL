using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FastSQL.Core.UI.Models
{
    public class MenuItemDefinition
    {
        public string Name { get; set; }
        public string CommandName { get; set; }
        public ObservableCollection<MenuItemDefinition> Children { get; set; }
    }
}
