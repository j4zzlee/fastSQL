using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Events
{
    public class DataGridCommandEventArgument
    {
        public string CommandName { get; set; }
        public IEnumerable<object> SelectedItems { get; set; }
    }
}
