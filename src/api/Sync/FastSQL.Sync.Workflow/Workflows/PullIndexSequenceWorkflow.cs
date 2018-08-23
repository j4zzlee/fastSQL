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
    [Description("Pull Indexes in Sequence Mode")]
    public class PullIndexSequenceWorkflow : BaseWorkflow<GeneralMessage>
    {
        public override string Id => nameof(PullIndexSequenceWorkflow);

        public override int Version => 1;

        public override bool IsGeneric => true;

        public PullIndexSequenceWorkflow()
        {
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
                    .Do(i => i.StartWith<Delay>(d => TimeSpan.FromMinutes(1)))
                    .If(s => s.Indexes != null && s.Indexes.Count() > 0)
                    .Do(i =>
                    {
                        i
                            .StartWith(ii => { })
                            .While(w => w.Counter < w.Indexes.Count())
                            .Do(dd => dd.StartWith<UpdateIndexChangesStep>()
                                .Input(u => u.IndexModel, g => g.Indexes.ElementAt(g.Counter))
                                .Input(u => u.Counter, w => w.Counter)
                                .Output(s => s.Counter, u => u.Counter))
                            .Then<Delay>(d => TimeSpan.FromSeconds(2));
                    })
                    .Then<Delay>(d => TimeSpan.FromSeconds(2));
                   ;
               });
        }
    }
}
