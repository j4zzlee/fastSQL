using FastSQL.Core;
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
using System.Text;
using WorkflowCore.Interface;
using WorkflowCore.Primitives;

namespace FastSQL.Sync.Workflow.Workflows
{
    [Description("Requeue Errors")]
    public class RequeueErrorsWorkflow : BaseWorkflow<GeneralMessage>
    {
        private readonly ILogger logger;
        private readonly EntityRepository entityRepository;
        private readonly AttributeRepository attributeRepository;
        private readonly ScheduleOptionRepository scheduleOptionRepository;
        private readonly WorkingSchedules workingSchedules;

        public override string Id => nameof(RequeueErrorsWorkflow);
        public override int Version => 1;
        public override bool IsGeneric => true;

        public RequeueErrorsWorkflow(
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            ScheduleOptionRepository scheduleOptionRepository,
            ResolverFactory resolverFactory,
            WorkingSchedules workingSchedules)
        {
            this.logger = resolverFactory.Resolve<ILogger>("Workflow");
            this.entityRepository = entityRepository;
            this.attributeRepository = attributeRepository;
            this.scheduleOptionRepository = scheduleOptionRepository;
            this.workingSchedules = workingSchedules;
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
                        .Do(i => i.StartWith<Delay>(d => TimeSpan.FromHours(1)))
                        .If(s => s.Indexes != null && s.Indexes.Count() > 0)
                        .Do(i =>
                        {
                            i
                                .StartWith(ii => { })
                                .While(w => w.Counter < w.Indexes.Count())
                                .Do(dd => dd.StartWith<RequeueErrorsStep>()
                                    .Input(u => u.IndexModel, g => g.Indexes.ElementAt(g.Counter))
                                    .Output(s => s.Counter, u => u.Counter))
                                .Then<Delay>(d => TimeSpan.FromHours(1));
                        });
               });
        }
    }
}
