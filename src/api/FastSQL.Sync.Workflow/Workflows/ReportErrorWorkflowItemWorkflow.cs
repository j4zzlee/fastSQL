using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using WorkflowCore.Interface;

namespace FastSQL.Sync.Workflow.Workflows
{
    [Description("Report Error Workflow Items")]
    public class ReportErrorWorkflowItemWorkflow : IWorkflow
    {
        public string Id => nameof(ReportErrorItemsWorkflow);

        public int Version => 1;

        public ReportErrorWorkflowItemWorkflow()
        {

        }

        public void Build(IWorkflowBuilder<object> builder)
        {
            builder.StartWith(x => { })
               .While(d => true)
               .Do(x => x.StartWith(s => {
                   Console.WriteLine("Report");
               }).Delay(d => TimeSpan.FromSeconds(10)));
        }
    }
}
