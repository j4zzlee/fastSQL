using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FastSQL.API.ViewModels
{
    public class CreateConnectionViewModel
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(int.MaxValue)]
        public string Description { get; set; }

        [Required]
        [MaxLength(255)]
        public string ProviderId { get; set; }

        [Required]
        public IEnumerable<OptionItem> Options { get; set; }
    }
}
