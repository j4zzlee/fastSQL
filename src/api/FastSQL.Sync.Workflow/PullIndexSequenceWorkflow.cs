using System;
using System.Collections.Generic;
using System.Text;
using WorkflowCore.Interface;

namespace FastSQL.Sync.Workflow
{
    public class PullIndexSequenceWorkflow : IWorkflow
    {
        public string Id => nameof(PullIndexSequenceWorkflow);

        public int Version => 1;

        public void Build(IWorkflowBuilder<object> builder)
        {
            builder.StartWith(x => { })
                .While(d => true)
                .Do(x => x.StartWith(s => Console.WriteLine("Pull Index Sequence")).Delay(d => TimeSpan.FromSeconds(1)));
        }
    }
}
