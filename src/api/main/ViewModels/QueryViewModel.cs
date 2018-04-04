using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace api.ViewModels
{
    public class QueryViewModel
    {
        public string RawQuery { get; set; }
        public List<OptionItem> Options { get; set; }
    }
}
