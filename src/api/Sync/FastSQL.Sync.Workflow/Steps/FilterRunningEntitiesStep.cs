using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using FastSQL.Sync.Core.Workflows;
using WorkflowCore.Interface;

namespace FastSQL.Sync.Workflow.Steps
{
    public class FilterRunningEntitiesStep : BaseStepBodyInvoker
    {
        private readonly WorkingSchedules workingSchedules;

        public string WorkflowId { get; set; }
        public IEnumerable<IIndexModel> Indexes { get; set; }
        public FilterRunningEntitiesStep(WorkingSchedules workingSchedules) : base()
        {
            this.workingSchedules = workingSchedules;
        }

        public override async Task Invoke(IStepExecutionContext context = null)
        {
            var scheduleOptionRepository = RepositoryFactory.Create<ScheduleOptionRepository>(this);
            var entityRepository = RepositoryFactory.Create<EntityRepository>(this);
            var attributeRepository = RepositoryFactory.Create<AttributeRepository>(this);
            try
            {
                await Task.Run(() =>
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
                    Indexes = entities
                        .Select(e => e as IIndexModel)
                        .Union(attributes.Select(a => a as IIndexModel));
                });
            }
            catch (Exception ex)
            {
                ErrorLogger.Error(ex, ex.Message);
                throw;
            }
            finally
            {
                entityRepository?.Dispose();
                attributeRepository?.Dispose();
                scheduleOptionRepository?.Dispose();
            }
        }
    }
}
