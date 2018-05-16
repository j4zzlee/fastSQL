using FastSQL.Sync.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FastSQL.Sync.Core.Models
{
    [Table("core_index_dependency")]
    public class DependencyItemModel
    {
        [Key]
        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        public EntityType EntityType { get; set; }
        public Guid TargetEntityId { get; set; }
        public EntityType TargetEntityType { get; set; }
        public IntegrationStep DependOnStep { get; set; } = IntegrationStep.Pushed;
        public IntegrationStep StepToExecute { get; set; } = IntegrationStep.Pushing;
        public bool ExecuteImmediately { get; set; } = false;
        public string ReferenceKeys { get; set; }
        public string ForeignKeys { get; set; }

        public bool HasDependOnStep(IntegrationStep step)
        {
            return (DependOnStep & step) > 0;
        }

        public bool HasExecutionStep(IntegrationStep step)
        {
            return (StepToExecute & step) > 0;
        }
    }
}
