using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Queuers;
using FastSQL.Sync.Core.Repositories;
using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace FastSQL.Sync.Workflow.Steps
{
    public class QueueIndexChangesStep : BaseStepBodyInvoker
    {
        public IIndexModel IndexModel { get; set; }
        private readonly EntityRepository entityRepository;
        private readonly AttributeRepository attributeRepository;
        private readonly ConnectionRepository connectionRepository;
        private readonly QueueChangesManager queueChangesManager;

        public QueueIndexChangesStep(ResolverFactory resolver,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            ConnectionRepository connectionRepository,
            QueueChangesManager queueChangesManager): base(resolver)
        {
            this.entityRepository = entityRepository;
            this.attributeRepository = attributeRepository;
            this.connectionRepository = connectionRepository;
            this.queueChangesManager = queueChangesManager;
        }

        public override async Task Invoke(IStepExecutionContext context = null)
        {
            Logger.Information($@"Queueing index changes of {IndexModel.Name}/{IndexModel.Id}...");
            queueChangesManager.SetIndex(IndexModel);
            queueChangesManager.OnReport(s => Logger.Information(s));
            await queueChangesManager.QueueChanges();
            Logger.Information($@"Queued index changes of {IndexModel.Name}/{IndexModel.Id}");
        }
    }
}
