using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Core
{
    public class OptionItem
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public OptionType Type { get; set; }
        public string Value { get; set; }
        public bool IsAdvanced { get; set; }
        public List<string> Source { get; set; }
    }
}
