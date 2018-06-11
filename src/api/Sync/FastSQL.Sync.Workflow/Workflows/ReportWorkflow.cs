using FastSQL.Sync.Workflow.Steps;
using System.ComponentModel;
using WorkflowCore.Interface;

namespace FastSQL.Sync.Workflow.Workflows
{
    [Description("Report")]
    public class ReportWorkflow : BaseWorkflow
    {
        public override string Id => nameof(ReportWorkflow);

        public override int Version => 1;

        public ReportWorkflow()
        {

        }

        public override void Build(IWorkflowBuilder<object> builder)
        {
            builder
                .StartWith(x => { })
                .Then<ReportStep>(p => p.Then(p)); // deal with it :)
        }
    }
}
