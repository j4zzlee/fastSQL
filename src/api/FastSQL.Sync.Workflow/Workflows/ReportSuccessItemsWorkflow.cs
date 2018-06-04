using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using WorkflowCore.Interface;

namespace FastSQL.Sync.Workflow.Workflows
{
    [Description("Report Sucess Items")]
    public class ReportSuccessItemsWorkflow : IWorkflow
    {
        public string Id => nameof(ReportSuccessItemsWorkflow);

        public int Version => 1;

        public ReportSuccessItemsWorkflow()
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
