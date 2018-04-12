using FastSQL.Sync.Core.Attributes;
using FastSQL.Sync.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastSQL.Sync.Core.Models
{
    [Table("core_connections")]
    [EntityType(EntityType.Connection)]
    public class ConnectionModel
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(int.MaxValue)]
        public string Description { get; set; }

        [MaxLength(255)]
        public string ProviderId { get; set; }

        [NotMapped]
        public EntityType EntityType => EntityType.Connection;
    }
}
