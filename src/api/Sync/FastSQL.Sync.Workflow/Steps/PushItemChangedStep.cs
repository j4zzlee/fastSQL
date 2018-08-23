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
        public PushItemChangedStep() : base()
        {
        }

        public override async Task Invoke(IStepExecutionContext context = null)
        {
            var entityRepository = ResolverFactory.Resolve<EntityRepository>();
            var attributeRepository = ResolverFactory.Resolve<AttributeRepository>();
            var messageRepository = ResolverFactory.Resolve<MessageRepository>();
            var queueItemRepository = ResolverFactory.Resolve<QueueItemRepository>();
            IIndexModel indexModel = null;
            IndexItemModel itemModel = null;
            var logger = ResolverFactory.Resolve<ILogger>("SyncService");
            var errorLogger = ResolverFactory.Resolve<ILogger>("Error");
            var pusherManager = ResolverFactory.Resolve<PusherManager>();
            try
            {
                var executeAt = DateTime.Now.ToUnixTimestamp();
                var firstQueuedItem = entityRepository.GetCurrentQueuedItems();
                if (firstQueuedItem == null)
                {
                    return;
                }

                if (firstQueuedItem.TargetEntityType == EntityType.Entity)
                {
                    indexModel = entityRepository.GetById(firstQueuedItem.TargetEntityId.ToString());
                }
                else
                {
                    indexModel = attributeRepository.GetById(firstQueuedItem.TargetEntityId.ToString());
                }
                
                pusherManager.SetIndex(indexModel);
                pusherManager.OnReport(s => logger.Information(s));

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
                errorLogger.Error(ex, ex.Message);
                throw;
            }
            finally
            {
                entityRepository?.Dispose();
                attributeRepository?.Dispose();
                messageRepository?.Dispose();
                queueItemRepository?.Dispose();
                ResolverFactory.Release(logger);
                ResolverFactory.Release(errorLogger);
                logger = null;
                errorLogger = null;
            }
        }
    }
}
