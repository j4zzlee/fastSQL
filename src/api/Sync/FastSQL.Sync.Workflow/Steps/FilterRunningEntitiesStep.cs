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
        private readonly ScheduleOptionRepository scheduleOptionRepository;
        private readonly EntityRepository entityRepository;
        private readonly AttributeRepository attributeRepository;

        public string WorkflowId { get; set; }
        public IEnumerable<IIndexModel> Indexes { get; set; }
        public FilterRunningEntitiesStep(ResolverFactory resolver,
            WorkingSchedules workingSchedules,
            ScheduleOptionRepository scheduleOptionRepository,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository) : base(resolver)
        {
            this.workingSchedules = workingSchedules;
            this.scheduleOptionRepository = scheduleOptionRepository;
            this.entityRepository = entityRepository;
            this.attributeRepository = attributeRepository;
        }

        public override async Task Invoke(IStepExecutionContext context = null)
        {
            try
            {
                await Task.Run(() => {
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
            }
        }
    }
}
