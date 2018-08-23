using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using FastSQL.Sync.Core.Workflows;
using Serilog;
using WorkflowCore.Interface;

namespace FastSQL.Sync.Workflow.Steps
{
    public class FilterRunningEntitiesStep : BaseStepBodyInvoker
    {
        private readonly WorkingSchedules workingSchedules;

        public string WorkflowId { get; set; }
        public List<IIndexModel> Indexes { get; set; }

        public FilterRunningEntitiesStep(WorkingSchedules workingSchedules) : base()
        {
            this.workingSchedules = workingSchedules;
        }

        public override async Task Invoke(IStepExecutionContext context = null)
        {
            var scheduleOptionRepository = ResolverFactory.Resolve<ScheduleOptionRepository>();
            var entityRepository = ResolverFactory.Resolve<EntityRepository>();
            var attributeRepository = ResolverFactory.Resolve<AttributeRepository>();
            var logger = ResolverFactory.Resolve<ILogger>("SyncService");
            var errorLogger = ResolverFactory.Resolve<ILogger>("Error");
            try
            {
                
                Indexes = await Task.Run(() =>
                {
                    var scheduleOptions = workingSchedules.GetWorkingSchedules() ?? scheduleOptionRepository
                        .GetByWorkflow(WorkflowId)
                        .Where(o => !o.IsParallel && o.Enabled);
                    var entities = entityRepository
                        .GetAll()
                        .Where(e => e.Enabled && scheduleOptions.Any(o => o.TargetEntityId == e.Id && o.TargetEntityType == e.EntityType));
                    var attributes = attributeRepository
                        .GetAll()
                        .Where(e => e.Enabled && scheduleOptions.Any(o => o.TargetEntityId == e.Id && o.TargetEntityType == e.EntityType));
                    return entities.Union(attributes.Select(a => a as IIndexModel)).ToList();
                });
            }
            catch (Exception ex)
            {
                errorLogger.Error(ex, ex.Message);
                throw;
            }
            finally
            {
                entityRepository?.Dispose();
                attributeRepository?.Dispose();
                scheduleOptionRepository?.Dispose();
                ResolverFactory.Release(logger);
                ResolverFactory.Release(errorLogger);
                logger = null;
                errorLogger = null;
            }
        }
    }
}
