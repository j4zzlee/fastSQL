using FastSQL.Sync.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastSQL.Sync.Core.Models
{
    [Table("beehexa_core_entities")]
    public class EntityModel
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProcessorId { get; set; }
        public Guid SourceConnectionId { get; set; }
        public Guid DestinationConnectionId { get; set; }
        public EntityState State { get; set; }

        [NotMapped]
        public EntityType EntityType => EntityType.Entity;
    }
}
