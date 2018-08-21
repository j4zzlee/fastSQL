using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Indexer;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using FastSQL.Sync.Core.Workflows;
using FastSQL.Sync.Workflow.Models;
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
    [Description("Pull Indexes in Parallel Mode")]
    public class PullIndexParallelWorkflow : BaseWorkflow<GeneralMessage>
    {
        private readonly EntityRepository entityRepository;
        private readonly AttributeRepository attributeRepository;
        private readonly ScheduleOptionRepository scheduleOptionRepository;
        private readonly IndexerManager indexerManager;
        private readonly WorkingSchedules workingSchedules;
        private readonly ILogger logger;

        public override string Id => nameof(PullIndexParallelWorkflow);

        public override int Version => 1;

        public override bool IsGeneric => true;

        public PullIndexParallelWorkflow(
           EntityRepository entityRepository,
           AttributeRepository attributeRepository,
           ScheduleOptionRepository scheduleOptionRepository,
           IndexerManager indexerManager,
           ResolverFactory resolverFactory,
           WorkingSchedules workingSchedules)
        {
            this.entityRepository = entityRepository;
            this.attributeRepository = attributeRepository;
            this.scheduleOptionRepository = scheduleOptionRepository;
            this.indexerManager = indexerManager;
            this.workingSchedules = workingSchedules;
            this.logger = resolverFactory.Resolve<ILogger>("Workflow");
        }

        public override void Build(IWorkflowBuilder<GeneralMessage> builder)
        {
            builder.StartWith(x => { })
               .While(d => true)
               .Do(x =>
               {
                   x.StartWith<FilterRunningEntitiesStep>()
                    .Input(s => s.WorkflowId, s => Id)
                    .Output(d => d.Indexes, d => d.Indexes)
                    .Output(d => d.Counter, d => 0)
                    .If(s => s.Indexes == null || s.Indexes.Count() <= 0)
                    .Do(i => i.StartWith<Delay>(d => TimeSpan.FromMinutes(10)))
                    .If(s => s.Indexes != null && s.Indexes.Count() > 0)
                    .Do(i =>
                    {
                        i
                            .StartWith(s => { })
                            .ForEach(ff => ff.Indexes)
                            .Do(dd =>
                            {
                                dd.StartWith<UpdateIndexChangesStep>()
                                .Input(s => s.IndexModel, (d, ctx) => ctx.Item);
                            })
                            .Delay(d => TimeSpan.FromSeconds(2));
                    });
               });
        }
    }
}
