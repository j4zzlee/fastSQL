using System;
using System.Collections.Generic;
using System.Text;
using WorkflowCore.Interface;

namespace FastSQL.Sync.Workflow
{
    public class PullIndexParallelWorkflow : IWorkflow
    {
        public string Id => nameof(PullIndexParallelWorkflow);

        public int Version => 1;

        public void Build(IWorkflowBuilder<object> builder)
        {
            builder.StartWith(x => { })
               .While(d => true)
               .Do(x => x.StartWith(s => Console.WriteLine("Pull Index Parallel")).Delay(d => TimeSpan.FromSeconds(3)));
        }
    }
}
