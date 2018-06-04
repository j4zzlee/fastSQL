using System;
using WorkflowCore.Interface;

namespace WorkflowCore.Extensions.WorkflowController
{
    public static class WorkflowControllerExtensions
    {
        public static void RegisterWorkflow(this IWorkflowHost host, IWorkflow workflow)
        {
            var reg = host.Registry;
            reg.RegisterWorkflow(workflow);
        }
    }
}
