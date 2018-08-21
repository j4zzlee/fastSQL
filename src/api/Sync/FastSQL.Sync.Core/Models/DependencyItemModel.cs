using FastSQL.Sync.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.RegularExpressions;

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
        public string ReferenceKeys { get; set; } // Keys that hold by TargetEntity
        public string ForeignKeys { get; set; } // Keys that hold by the Entity

        [NotMapped]
        public string[] ForeignKeysArr => string.IsNullOrWhiteSpace(ForeignKeys) 
            ? new string[] { } 
            : Regex.Split(ForeignKeys, "[,;|]", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        [NotMapped]
        public string[] ReferenceKeysArr => string.IsNullOrWhiteSpace(ReferenceKeys)
          ? new string[] { }
          : Regex.Split(ReferenceKeys, "[,;|]", RegexOptions.Multiline | RegexOptions.IgnoreCase);

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
