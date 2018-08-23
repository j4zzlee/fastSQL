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
using System.Timers;
using WorkflowCore.Extensions.WorkflowController;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace FastSQL.Sync.Workflow
{
    public class SyncService: IDisposable
    {
        private IWorkflowHost _host;
        public ResolverFactory ResolverFactory { get; set; }
        private readonly IEnumerable<IBaseWorkflow> workflows;
        private readonly WorkingSchedules workingSchedules;
        private ILogger _logger;
        private ILogger Logger => _logger ?? (_logger = ResolverFactory.Resolve<ILogger>("SyncService"));
        private ILogger _errorLogger;
        private ILogger ErrorLogger => _errorLogger ?? (_errorLogger = ResolverFactory.Resolve<ILogger>("Error"));
        private bool _running;
        private bool isRegistered;
        private IList<string> _ranWorkflows;

        public SyncService(
            IWorkflowHost host,
            IEnumerable<IBaseWorkflow> workflows,
            WorkingSchedules workingSchedules)
        {
            _ranWorkflows = new List<string>();
            _host = host;
            this.workflows = workflows;
            this.workingSchedules = workingSchedules;
        }

        public virtual void Dispose()
        {
            
        }

        private void RegisterWorkflows()
        {
            if (isRegistered)
            {
                return;
            }
            //var savedWorkflows = _host.PersistenceStore.GetWorkflowInstances(null, null, null, null, 0, 100);
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
                    ErrorLogger.Error("The workflow is running");
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
                        if (workflow.IsGeneric)
                        {
                            _host.StartGenericWorkflow(workflow);
                        }
                        else
                        {
                            _host.StartWorkflow(workflow.Id);
                        }
                       
                    }
                    else
                    {
                        _host.ResumeWorkflow(workflow.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Error(ex, "Sync Service failed to run.");
                throw;
            }
        }
        
        private void _host_OnStepError(WorkflowInstance workflow, WorkflowStep step, Exception exception)
        {
            ErrorLogger.Error(exception, $@"Workflow {workflow.WorkflowDefinitionId}/{workflow.Id} has raised an error on step {step.Name}/{step.Id}");
        }

        public void Stop()
        {
            try
            {
                Logger.Information("Service stopped.");
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
            var entityRepository = ResolverFactory.Resolve<EntityRepository>();
            var attributeRepository = ResolverFactory.Resolve<AttributeRepository>();
            try
            {
                if (_running)
                {
                    ErrorLogger.Error("The workflow is running");
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
                    if (workflow.IsGeneric)
                    {
                        _host.StartGenericWorkflow(workflow);
                    }
                    else
                    {
                        _host.StartWorkflow(workflow.Id);
                    }
                }
                else
                {
                    _host.ResumeWorkflow(workflow.Id);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Error(ex, "Sync Service failed to run.");
                throw;
            }
            finally
            {
                entityRepository?.Dispose();
                attributeRepository?.Dispose();
            }
        }
    }
}
