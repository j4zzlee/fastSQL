using System;
using System.Collections.Generic;
using System.Text;
using WorkflowCore.Interface;

namespace FastSQL.Sync.Workflow
{
    public class QueueIndexChangesWorkflow : IWorkflow
    {
        public string Id => nameof(QueueIndexChangesWorkflow);

        public int Version => 1;

        public void Build(IWorkflowBuilder<object> builder)
        {
            builder.StartWith(x => { })
               .While(d => true)
               .Do(x => x.StartWith(s => Console.WriteLine("Queue Index Changes")).Delay(d => TimeSpan.FromSeconds(5)));
        }
    }
}
