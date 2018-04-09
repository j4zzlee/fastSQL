using FastSQL.Sync.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FastSQL.Sync.Core.Models
{
    public class OptionModel
    {
        [Key]
        public Guid Id { get; set; }

        public Guid EntityId { get; set; }
        [MaxLength(255)]
        public EntityEnum EntityType { get; set; }

        [MaxLength(255)]
        public string Key { get; set; }
        [MaxLength(4000)]
        public string Value { get; set; }
    }
}
