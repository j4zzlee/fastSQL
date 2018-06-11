using FastSQL.Sync.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastSQL.Sync.Core.Models
{
    [Table("core_options")]
    public class OptionModel
    {
        [Key]
        public Guid Id { get; set; }

        public string EntityId { get; set; }
        
        public EntityType EntityType { get; set; }

        [MaxLength(255)]
        public string Key { get; set; }
        [MaxLength(4000)]
        public string Value { get; set; }
    }
}
