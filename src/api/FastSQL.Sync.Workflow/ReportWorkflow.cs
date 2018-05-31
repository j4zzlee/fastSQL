using System;
using System.Collections.Generic;
using System.Text;
using WorkflowCore.Interface;

namespace FastSQL.Sync.Workflow
{
    public class ReportWorkflow : IWorkflow
    {
        public string Id => nameof(ReportWorkflow);

        public int Version => 1;

        public void Build(IWorkflowBuilder<object> builder)
        {
            builder.StartWith(x => { })
               .While(d => true)
               .Do(x => x.StartWith(s => Console.WriteLine("Report")).Delay(d => TimeSpan.FromSeconds(10)));
        }
    }
}
