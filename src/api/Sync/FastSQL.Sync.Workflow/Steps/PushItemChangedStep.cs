using DateTimeExtensions;
using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Indexer;
using FastSQL.Sync.Core.Models;
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
    public class PushItemChangedStep : BaseStepBodyInvoker
    {
        private readonly IEnumerable<IEntityIndexer> entityIndexers;
        private readonly IEnumerable<IAttributeIndexer> attributeIndexers;
        private readonly IEnumerable<IEntityPusher> entityPushers;
        private readonly IEnumerable<IAttributePusher> attributePushers;
        private readonly EntityRepository entityRepository;
        private readonly AttributeRepository attributeRepository;
        private readonly MessageRepository messageRepository;
        private readonly QueueItemRepository queueItemRepository;
        private readonly ConnectionRepository connectionRepository;
        private readonly PusherManager pusherManager;
        public PushItemChangedStep(
            IEnumerable<IEntityIndexer> entityIndexers,
            IEnumerable<IAttributeIndexer> attributeIndexers,
            IEnumerable<IEntityPusher> entityPushers,
            IEnumerable<IAttributePusher> attributePushers,
            ResolverFactory resolver,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            MessageRepository messageRepository,
            QueueItemRepository queueItemRepository,
            ConnectionRepository connectionRepository,
            PusherManager pusherManager) : base(resolver)
        {
            this.entityIndexers = entityIndexers;
            this.attributeIndexers = attributeIndexers;
            this.entityPushers = entityPushers;
            this.attributePushers = attributePushers;
            this.entityRepository = entityRepository;
            this.attributeRepository = attributeRepository;
            this.messageRepository = messageRepository;
            this.queueItemRepository = queueItemRepository;
            this.connectionRepository = connectionRepository;
            this.pusherManager = pusherManager;
        }

        public override async Task Invoke(IStepExecutionContext context = null)
        {
            try
            {
                var executeAt = DateTime.Now.ToUnixTimestamp();
                var firstQueuedItem = entityRepository.GetCurrentQueuedItems();
                if (firstQueuedItem == null)
                {
                    return;
                }
                IIndexModel indexModel = null;
                IndexItemModel itemModel = null;
                IIndexer indexer = null;
                IPusher pusher = null;
                IEnumerable<OptionItem> options = null;
                if (firstQueuedItem.TargetEntityType == EntityType.Entity)
                {
                    indexModel = entityRepository.GetById(firstQueuedItem.TargetEntityId.ToString());
                    options = entityRepository.LoadOptions(indexModel.Id.ToString()).Select(o => new OptionItem { Name = o.Key, Value = o.Value });
                    var sourceConnection = connectionRepository.GetById(indexModel.SourceConnectionId.ToString());
                    var destinationConnection = connectionRepository.GetById(indexModel.DestinationConnectionId.ToString());
                    indexer = entityIndexers.FirstOrDefault(i => i.IsImplemented(indexModel.SourceProcessorId, sourceConnection.ProviderId));
                    pusher = entityPushers.FirstOrDefault(p => p.IsImplemented(indexModel.DestinationProcessorId, destinationConnection.ProviderId));
                }
                else
                {
                    var attributeModel = attributeRepository.GetById(firstQueuedItem.TargetEntityId.ToString());
                    indexModel = attributeModel;
                    var entityModel = entityRepository.GetById(attributeModel.EntityId.ToString());
                    options = attributeRepository.LoadOptions(attributeModel.Id.ToString()).Select(o => new OptionItem { Name = o.Key, Value = o.Value });
                    var sourceConnection = connectionRepository.GetById(attributeModel.SourceConnectionId.ToString());
                    var destinationConnection = connectionRepository.GetById(attributeModel.DestinationConnectionId.ToString());
                    indexer = attributeIndexers.FirstOrDefault(i => i.IsImplemented(attributeModel.SourceProcessorId, entityModel.SourceProcessorId, sourceConnection.ProviderId));
                    pusher = attributePushers.FirstOrDefault(p => p.IsImplemented(attributeModel.DestinationProcessorId, entityModel.DestinationProcessorId, destinationConnection.ProviderId));
                }

                indexer.SetIndex(indexModel);
                indexer.SetOptions(options);
                pusher.SetIndex(indexModel);
                pusher.SetOptions(options);
                pusherManager.SetIndex(indexModel);
                pusherManager.OnReport(s => Logger.Information(s));
                pusherManager.SetIndexer(indexer);
                pusherManager.SetPusher(pusher);

                try
                {
                    itemModel = entityRepository.GetIndexedItemById(indexModel, firstQueuedItem.TargetItemId.ToString());
                    var pushState = await pusherManager.PushItem(itemModel);
                    var messageId = messageRepository.Create(new
                    {
                        Message = string.Join("\n", pusherManager.GetReportMessages()),
                        CreatedAt = DateTime.Now.ToUnixTimestamp(),
                        MessageType = MessageType.Information,
                        Status = MessageStatus.None
                    });
                    queueItemRepository.Update(firstQueuedItem.Id.ToString(), new
                    {
                        UpdatedAt = DateTime.Now.ToUnixTimestamp(),
                        ExecuteAt = executeAt,
                        ExecutedAt = DateTime.Now.ToUnixTimestamp(),
                        MessageId = messageId,
                        Status = pushState
                    });
                }
                catch (Exception ex)
                {
                    var messages = $@"Queue item (Id: {firstQueuedItem.Id}) failed to run. 
Addtional information:
```{JsonConvert.SerializeObject(indexModel, Formatting.Indented)}```
Progress: 
```{string.Join("\n - ", pusherManager.GetReportMessages())}```
Exception: 
```{ex}```";
                    var messageId = messageRepository.Create(new
                    {
                        Message = messages,
                        CreatedAt = DateTime.Now.ToUnixTimestamp(),
                        MessageType = MessageType.Error,
                        Status = MessageStatus.None
                    });

                    queueItemRepository.Update(firstQueuedItem.Id.ToString(), new
                    {
                        UpdatedAt = DateTime.Now.ToUnixTimestamp(),
                        ExecuteAt = executeAt,
                        ExecutedAt = DateTime.Now.ToUnixTimestamp(),
                        MessageId = messageId,
                        Status = PushState.UnexpectedError
                    });
                    throw;
                }
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
