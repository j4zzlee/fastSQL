using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Indexer;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Puller;
using FastSQL.Sync.Core.Pusher;
using FastSQL.Sync.Core.Repositories;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace FastSQL.Sync.Workflow.Steps
{
    public class UpdateIndexChangesStep : BaseStepBodyInvoker
    {
        public IIndexModel IndexModel { get; set; }
        private readonly EntityRepository entityRepository;
        private readonly AttributeRepository attributeRepository;
        private readonly ConnectionRepository connectionRepository;
        private readonly IEnumerable<IPuller> pullers;
        private readonly IEnumerable<IIndexer> indexers;
        private readonly IndexerManager indexerManager;

        public UpdateIndexChangesStep(
            ResolverFactory resolver,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            ConnectionRepository connectionRepository,
            IEnumerable<IPuller> pullers,
            IEnumerable<IIndexer> indexers,
            IndexerManager indexerManager) : base(resolver)
        {
            this.entityRepository = entityRepository;
            this.attributeRepository = attributeRepository;
            this.connectionRepository = connectionRepository;
            this.pullers = pullers;
            this.indexers = indexers;
            this.indexerManager = indexerManager;
        }

        public override async Task Invoke(IStepExecutionContext context = null)
        {
            try
            {
                Logger.Information($@"Updating index changes of {IndexModel.Name}/{IndexModel.Id}...");
                var sourceConnection = connectionRepository.GetById(IndexModel.SourceConnectionId.ToString());
                //var destConnection = connectionRepository.GetById(IndexModel.DestinationConnectionId.ToString());
                IPuller puller = null;
                IIndexer indexer = null;
                if (IndexModel.EntityType == EntityType.Entity)
                {
                    puller = pullers.Where(p => typeof(IEntityPuller).IsAssignableFrom(p.GetType()))
                            .Select(p => (IEntityPuller)p)
                            .FirstOrDefault(p => p.IsImplemented(
                                IndexModel?.SourceProcessorId,
                                sourceConnection?.ProviderId));

                    indexer = indexers.Where(p => typeof(IEntityIndexer).IsAssignableFrom(p.GetType()))
                            .Select(p => (IEntityIndexer)p)
                            .FirstOrDefault(p => p.IsImplemented(
                                IndexModel?.SourceProcessorId,
                                sourceConnection?.ProviderId));
                }
                else
                {
                    var entity = entityRepository.GetById((IndexModel as AttributeModel).EntityId.ToString());
                    puller = pullers.Where(p => typeof(IAttributePuller).IsAssignableFrom(p.GetType()))
                            .Select(p => (IAttributePuller)p)
                            .FirstOrDefault(p => p.IsImplemented(
                                IndexModel.SourceProcessorId,
                                entity?.SourceProcessorId,
                                sourceConnection?.ProviderId));

                    indexer = indexers.Where(p => typeof(IAttributeIndexer).IsAssignableFrom(p.GetType()))
                            .Select(p => (IAttributeIndexer)p)
                            .FirstOrDefault(p => p.IsImplemented(
                                IndexModel.SourceProcessorId,
                                entity?.SourceProcessorId,
                                sourceConnection?.ProviderId));
                }
                var options = entityRepository.LoadOptions(IndexModel.Id.ToString(), IndexModel.EntityType)
                    .Select(o => new OptionItem { Name = o.Key, Value = o.Value });
                puller.SetIndex(IndexModel);
                puller.SetOptions(options);
                indexer.SetIndex(IndexModel);
                indexer.SetOptions(options);
                indexerManager.SetIndex(IndexModel);
                indexerManager.SetPuller(puller);
                indexerManager.SetIndexer(indexer);
                indexerManager.OnReport(s =>
                {
                    Logger.Information(s);
                });

                await indexerManager.PullNext();
                Logger.Information($@"Updated index changes of {IndexModel.Name}/{IndexModel.Id}");
            }
            catch (Exception ex)
            {
                ErrorLogger.Error(ex, ex.Message);
                throw;
            }
        }
    }
}
