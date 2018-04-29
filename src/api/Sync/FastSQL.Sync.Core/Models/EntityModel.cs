using FastSQL.Sync.Core.Attributes;
using FastSQL.Sync.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastSQL.Sync.Core.Models
{
    [Table("core_entities")]
    [EntityType(EntityType.Entity)]
    public class EntityModel
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SourceProcessorId { get; set; }
        public string DestinationProcessorId { get; set; }
        public Guid SourceConnectionId { get; set; }
        public Guid DestinationConnectionId { get; set; }
        public EntityState State { get; set; }

        [NotMapped]
        public EntityType EntityType => EntityType.Entity;

        public void AddState(EntityState state)
        {
            State = State != 0 ? (State | state) : state;
        }

        public void RemoveState(EntityState state)
        {
            if (State == 0)
            {
                return;
            }

            State = (State | state) ^ state;
        }

        public bool HasState(EntityState state)
        {
            return (State & state) > 0;
        }
    }
}
