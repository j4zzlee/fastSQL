using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FastSQL.Core
{
    public class OptionItem
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(int.MaxValue)]
        public string DisplayName { get; set; }
        [NotMapped]
        public string Example { get; set; }
        [NotMapped]
        public string Description { get; set; }
        public OptionType Type { get; set; }
        [MaxLength(int.MaxValue)]
        public string Value { get; set; }
        public List<string> OptionGroupNames { get; set; }
        public List<string> Source { get; set; }
    }
}
