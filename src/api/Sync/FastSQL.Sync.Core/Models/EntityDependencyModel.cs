using FastSQL.Sync.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FastSQL.Sync.Core.Models
{
    [Table("core_entity_dependency")]
    public class EntityDependencyModel
    {
        [Key]
        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        public EntityType EntityType { get; set; }
        public Guid TargetEntityId { get; set; }
        public EntityType TargetEntityType { get; set; }
        public IntegrationStep DependOnStep { get; set; } = IntegrationStep.Push;
        public IntegrationStep StepToExecute { get; set; } = IntegrationStep.Push;
        public bool ExecuteImmediately { get; set; } = false;
    }
}
