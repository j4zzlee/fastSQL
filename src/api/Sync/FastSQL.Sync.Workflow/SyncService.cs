using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Workflows;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using WorkflowCore.Extensions.WorkflowController;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace FastSQL.Sync.Workflow
{
    public class SyncService: IDisposable
    {
        private readonly IWorkflowHost _host;
        private readonly IEnumerable<INormalWorkflow> workflows;
        private readonly IEnumerable<IGenericWorkflow> genericWorkflows;
        private ILogger _logger;
        private ILogger _errorLogger;
        public SyncService(
            ResolverFactory resolverFactory,
            IWorkflowHost host, 
            IEnumerable<INormalWorkflow> workflows,
            IEnumerable<IGenericWorkflow> genericWorkflows)
        {
            _logger = resolverFactory.Resolve<ILogger>("SyncService");
            _errorLogger = resolverFactory.Resolve<ILogger>("Error");
            _host = host;
            this.workflows = workflows;
            this.genericWorkflows = genericWorkflows;
        }

        public void Dispose()
        {
            //_logger.Information("Service stopped.");
            //_host.Stop();
        }

        public void Start()
        {
            try
            {
                foreach (var workflow in workflows)
                {
                    _host.RegisterWorkflow(workflow as IWorkflow);
                }

                foreach (var workflow in genericWorkflows)
                {
                    _host.RegisterGenericWorkflow(workflow);
                }

                _host.Start();
                _host.OnStepError += _host_OnStepError;

                foreach (var workflow in workflows)
                {
                    _host.StartWorkflow(workflow.Id, workflow.Version, null);
                }

                foreach (var workflow in genericWorkflows)
                {
                    _host.StartWorkflow(workflow.Id, workflow.Version, null);
                }
            }
            catch (Exception ex)
            {
                _errorLogger.Error(ex, "Sync Service failed to run.");
                throw;
            }
        }

        private void _host_OnStepError(WorkflowInstance workflow, WorkflowStep step, Exception exception)
        {
            _errorLogger.Error(exception, $@"Workflow {workflow.WorkflowDefinitionId}/{workflow.Id} has raised an error on step {step.Name}/{step.Id}");
        }

        public void Stop()
        {
            _logger.Information("Service stopped.");
            _host.Stop();
        }
    }
}
