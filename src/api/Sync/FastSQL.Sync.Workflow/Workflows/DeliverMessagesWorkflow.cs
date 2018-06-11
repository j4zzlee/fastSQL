using FastSQL.Sync.Workflow.Steps;
using System.ComponentModel;
using WorkflowCore.Interface;

namespace FastSQL.Sync.Workflow.Workflows
{
    [Description("Deliver Messages")]
    public class DeliverMessagesWorkflow : BaseWorkflow
    {
        public override string Id => nameof(DeliverMessagesWorkflow);

        public override int Version => 1;

        public DeliverMessagesWorkflow()
        {

        }

        public override void Build(IWorkflowBuilder<object> builder)
        {
            builder
                 .StartWith(x => { })
                 .Then<DeliverMessageStep>(p => p.Then(p)); // deal with it :)
        }
    }
}
