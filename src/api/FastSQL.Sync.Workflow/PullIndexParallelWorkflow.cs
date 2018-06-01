using FastSQL.Sync.Core.Indexer;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using WorkflowCore.Interface;

namespace FastSQL.Sync.Workflow
{
    public class PullIndexParallelWorkflow : IWorkflow
    {
        private readonly EntityRepository entityRepository;
        private readonly AttributeRepository attributeRepository;
        private readonly IndexerManager indexerManager;

        public string Id => nameof(PullIndexParallelWorkflow);

        public int Version => 1;

        public PullIndexParallelWorkflow(
           EntityRepository entityRepository,
           AttributeRepository attributeRepository,
           IndexerManager indexerManager)
        {
            this.entityRepository = entityRepository;
            this.attributeRepository = attributeRepository;
            this.indexerManager = indexerManager;
        }

        public void Build(IWorkflowBuilder<object> builder)
        {
            builder.StartWith(x => { })
               .While(d => true)
               .Do(x => x.StartWith(s => Console.WriteLine("Pull Index Parallel")).Delay(d => TimeSpan.FromSeconds(3)));
        }
    }
}
