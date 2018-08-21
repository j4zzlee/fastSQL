using FastSQL.Sync.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FastSQL.App.ViewModels
{
    public class DependencyItemViewModel
    {
        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        public EntityType EntityType { get; set; }
        public Guid TargetEntityId { get; set; }
        public EntityType TargetEntityType { get; set; }
        public IntegrationStep DependOnStep { get; set; } = IntegrationStep.Pushing;
        public IntegrationStep StepToExecute { get; set; } = IntegrationStep.Pushing;
        public bool ExecuteImmediately { get; set; } = false;
        public string ReferenceKeys { get; set; }
        public string ForeignKeys { get; set; }
        
        [NotMapped]
        public string[] ForeignKeysArr => string.IsNullOrWhiteSpace(ForeignKeys)
            ? new string[] { }
            : Regex.Split(ForeignKeys, "[,;|]", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        [NotMapped]
        public string[] ReferenceKeysArr => string.IsNullOrWhiteSpace(ReferenceKeys)
          ? new string[] { }
          : Regex.Split(ReferenceKeys, "[,;|]", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        
        public string ExecuteImmediatelyStr => ExecuteImmediately ? "Yes" : "No";
        public string DependOnStepStr => DependOnStep.ToString();
        public string StepToExecuteStr => StepToExecute.ToString();
        public string DependOn { get; set; }
        public string TargetTypeStr => TargetEntityType.ToString();

    }
}
