using FastSQL.Core;
using FastSQL.Sync.Workflow.Steps;
using Serilog;
using System;
using System.ComponentModel;
using WorkflowCore.Interface;

namespace FastSQL.Sync.Workflow.Workflows
{
    [Description("Pull Item")]
    public class PushWorkflow : BaseWorkflow
    {
        private readonly ILogger logger;
        public override string Id => nameof(PushWorkflow);

        public override int Version => 1;

        public PushWorkflow(ResolverFactory resolverFactory)
        {
            this.logger = resolverFactory.Resolve<ILogger>("Workflow");
        }

        public override void Build(IWorkflowBuilder<object> builder)
        {
            builder
                .StartWith(x => { })
                .Then<PushItemChangedStep>(p => p.Delay(d => TimeSpan.FromMilliseconds(500)).Then(p)); // deal with it :)
        }
    }
}
