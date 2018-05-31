using FastSQL.Core;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace FastSQL.Sync.Workflow.Steps
{
    public class GetConfigurationsStep : StepBody
    {
        public IEnumerable<object> Configurations { get; set; }

        private ILogger _logger;
        public GetConfigurationsStep(ResolverFactory resolver)
        {
            _logger = resolver.Resolve<ILogger>("Workflow");
        }
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            Configurations = new List<string> { "Ahihi" };
            _logger.Information("Hi there!");
            return ExecutionResult.Next();
        }
    }
}
