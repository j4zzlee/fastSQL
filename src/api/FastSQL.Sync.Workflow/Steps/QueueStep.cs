using FastSQL.Core;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace FastSQL.Sync.Workflow.Steps
{
    public class QueueStep : StepBody
    {
        private ILogger _logger;

        public QueueStep(ResolverFactory resolver)
        {
            _logger = resolver.Resolve<ILogger>("Workflow");
        }
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            _logger.Information("Goodbye!");
            return ExecutionResult.Next();
        }
    }
}
