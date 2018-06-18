using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Indexer;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using FastSQL.Sync.Workflow.Steps;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using WorkflowCore.Interface;
using WorkflowCore.Primitives;

namespace FastSQL.Sync.Workflow.Workflows
{
    [Description("Pull Indexes in Sequence Mode")]
    public class PullIndexSequenceWorkflow : BaseWorkflow
    {
        private readonly EntityRepository entityRepository;
        private readonly AttributeRepository attributeRepository;
        private readonly ScheduleOptionRepository scheduleOptionRepository;
        private readonly IndexerManager indexerManager;
        private readonly ILogger logger;

        public override string Id => nameof(PullIndexSequenceWorkflow);

        public override int Version => 1;

        public PullIndexSequenceWorkflow(
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            ScheduleOptionRepository scheduleOptionRepository,
            IndexerManager indexerManager,
            ResolverFactory resolverFactory)
        {
            this.entityRepository = entityRepository;
            this.attributeRepository = attributeRepository;
            this.scheduleOptionRepository = scheduleOptionRepository;
            this.indexerManager = indexerManager;
            this.logger = resolverFactory.Resolve<ILogger>("Workflow");
        }

        public override void Build(IWorkflowBuilder<object> builder)
        {
            builder.StartWith(x => { })
               .While(d => true)
               .Do(x =>
               {
                   IEnumerable<IIndexModel> indexes = null;
                   //if ((Mode & WorkflowMode.Test) > 0 && IndexModel != null)
                   //{
                   //    indexes = new List<IIndexModel> { IndexModel };
                   //}
                   //else
                   //{
                   //    var scheduleOptions = scheduleOptionRepository
                   //     .GetByWorkflow(Id)
                   //     .Where(o => !o.IsParallel && o.Enabled);
                   //    var entities = entityRepository
                   //        .GetAll()
                   //        .Where(e => e.Enabled && scheduleOptions.Any(o => o.TargetEntityId == e.Id && o.TargetEntityType == e.EntityType));
                   //    var attributes = attributeRepository
                   //        .GetAll()
                   //        .Where(e => e.Enabled && scheduleOptions.Any(o => o.TargetEntityId == e.Id && o.TargetEntityType == e.EntityType));
                   //    indexes = entities
                   //        .Select(e => e as IIndexModel)
                   //        .Union(attributes.Select(a => a as IIndexModel));
                   //}
                   /**
                    * WORKFLOW is pre-built, we can only test via settings Enable/Disable in the form
                    **/
                   var scheduleOptions = scheduleOptionRepository
                        .GetByWorkflow(Id)
                        .Where(o => !o.IsParallel && o.Enabled);
                   var entities = entityRepository
                       .GetAll()
                       .Where(e => e.Enabled && scheduleOptions.Any(o => o.TargetEntityId == e.Id && o.TargetEntityType == e.EntityType));
                   var attributes = attributeRepository
                       .GetAll()
                       .Where(e => e.Enabled && scheduleOptions.Any(o => o.TargetEntityId == e.Id && o.TargetEntityType == e.EntityType));
                   indexes = entities
                       .Select(e => e as IIndexModel)
                       .Union(attributes.Select(a => a as IIndexModel));

                   if (indexes == null || indexes.Count() <= 0)
                   {
                       x.StartWith(c => { })
                        .Delay(d => TimeSpan.FromMinutes(10));
                   }
                   else
                   {
                       IStepBuilder<object, UpdateIndexChangesStep> ip = null;
                       foreach (var i in indexes)
                       {
                           if (ip == null)
                           {
                               ip = x.StartWith<UpdateIndexChangesStep>()
                                    .Input(s => s.IndexModel, d => i);
                           }
                           else
                           {
                               ip = ip.Then<UpdateIndexChangesStep>()
                                   .Input(s => s.IndexModel, d => i);
                           }
                       }
                   }
               });
        }
    }
}
