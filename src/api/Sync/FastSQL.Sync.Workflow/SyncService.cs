using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using FastSQL.Sync.Core.Workflows;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkflowCore.Extensions.WorkflowController;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace FastSQL.Sync.Workflow
{
    public class SyncService: IDisposable
    {
        private IWorkflowHost _host;
        private readonly ResolverFactory resolverFactory;
        private readonly EntityRepository entityRepository;
        private readonly AttributeRepository attributeRepository;
        private readonly IEnumerable<IBaseWorkflow> workflows;
        private readonly WorkingSchedules workingSchedules;
        private ILogger _logger;
        private ILogger _errorLogger;
        private bool _running;
        private bool isRegistered;
        private IList<string> _ranWorkflows;

        public SyncService(
            ResolverFactory resolverFactory,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            IWorkflowHost host,
            IEnumerable<IBaseWorkflow> workflows,
            WorkingSchedules workingSchedules)
        {
            _ranWorkflows = new List<string>();
            _logger = resolverFactory.Resolve<ILogger>("SyncService");
            _errorLogger = resolverFactory.Resolve<ILogger>("Error");
            this.resolverFactory = resolverFactory;
            this.entityRepository = entityRepository;
            this.attributeRepository = attributeRepository;
            _host = host;
            this.workflows = workflows;
            this.workingSchedules = workingSchedules;
            //RegisterWorkflows();
        }

        public void Dispose()
        {
            //_logger.Information("Service stopped.");
            //_host.Stop();
        }

        private void RegisterWorkflows()
        {
            if (isRegistered)
            {
                return;
            }

            foreach (var workflow in workflows.Where(w => !w.IsGeneric))
            {
                _host.RegisterWorkflow(workflow as IWorkflow);
            }

            foreach (var workflow in workflows.Where(w => w.IsGeneric))
            {
                _host.RegisterGenericWorkflow(workflow);
            }

            _host.OnStepError += _host_OnStepError;
            isRegistered = true;
        }
        
        public void Start()
        {
            try
            {
                if (_running)
                {
                    _errorLogger.Error("The workflow is running");
                    return;
                }
                _running = true;
                workingSchedules.SetSchedules(null);
                RegisterWorkflows();
                _host.Start();

                foreach (var workflow in workflows)
                {
                    if (!_ranWorkflows.Contains(workflow.Id))
                    {
                        _ranWorkflows.Add(workflow.Id);
                        _host.StartWorkflow(workflow.Id, workflow.Version, null);
                    }
                    else
                    {
                        _host.ResumeWorkflow(workflow.Id);
                    }
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
            try
            {
                _logger.Information("Service stopped.");
                _host.Stop();
            }
            finally
            {
                //resolverFactory.Release(_host);
                //_host = null;
                _running = false;
            }
        }

        public void StartTest(ScheduleOptionModel option)
        {
            try
            {
                if (_running)
                {
                    _errorLogger.Error("The workflow is running");
                    return;
                }
                _running = true;
                workingSchedules.SetSchedules(new List<ScheduleOptionModel> { option });
                RegisterWorkflows();
                var workflow = workflows.FirstOrDefault(w => w.Id == option.WorkflowId) as IBaseWorkflow;
                IIndexModel indexModel = option.TargetEntityType == EntityType.Entity
                        ? entityRepository.GetById(option.TargetEntityId.ToString()) as IIndexModel
                        : attributeRepository.GetById(option.TargetEntityId.ToString());
                
                if (indexModel == null)
                {
                    throw new Exception($"Could not find Index with ID: {option.TargetEntityId}, Name: {option.TargetEntityName}, Type: {option.TargetEntityType}");
                }
                
                _host.Start();
                if (!_ranWorkflows.Contains(workflow.Id))
                {
                    _ranWorkflows.Add(workflow.Id);
                    _host.StartWorkflow(workflow.Id, workflow.Version, null);
                }
                else
                {
                    _host.ResumeWorkflow(workflow.Id);
                }
            }
            catch (Exception ex)
            {
                _errorLogger.Error(ex, "Sync Service failed to run.");
                throw;
            }
        }
    }
}
