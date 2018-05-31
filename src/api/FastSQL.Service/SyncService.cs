using FastSQL.Core.Loggers;
using FastSQL.Sync.Workflow;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace FastSQL.Service
{
    public class SyncService: IDisposable
    {
        private readonly IWorkflowHost _host;
        private readonly IEnumerable<IWorkflow> workflows;
        private ILogger _logger;
        public SyncService(LoggerFactory loggerFactory, IWorkflowHost host, IEnumerable<IWorkflow> workflows)
        {
            _logger = loggerFactory
              .WriteToFile("SyncService")
              .WriteToConsole()
              .CreateApplicationLogger();
            _host = host;
            this.workflows = workflows;
        }

        public void Dispose()
        { 
        }

        public void Start()
        {
            try
            {
                foreach (var workflow in workflows)
                {
                    _host.RegisterWorkflow(workflow);
                }

                _host.Start();

                foreach (var workflow in workflows)
                {
                    _host.StartWorkflow(workflow.Id, workflow.Version, null);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Sync Service failed to run.");
                throw;
            }
        }

        public void Stop()
        {
            _host.Stop();
            _logger.Information("Service stopped.");
        }
    }
}
