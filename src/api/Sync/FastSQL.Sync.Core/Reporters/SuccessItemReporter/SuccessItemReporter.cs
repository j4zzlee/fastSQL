using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DateTimeExtensions;
using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Reporters;
using FastSQL.Sync.Core.Repositories;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.Slack;

namespace FastSQL.Sync.Core.Reporters
{
    public class SuccessItemReporter : BaseReporter
    {
        private readonly MessageRepository messageRepository;
        private readonly QueueItemRepository queueItemRepository;
        private readonly EntityRepository entityRepository;
        private readonly AttributeRepository attributeRepository;

        public SuccessItemReporter(
            ResolverFactory resolverFactory,
            SuccessItemReporterOptionManager optionManager,
            MessageRepository messageRepository,
            QueueItemRepository queueItemRepository,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            MessageDeliveryChannelRepository messageDeliveryChannelRepository
            ) : base(optionManager, resolverFactory, messageDeliveryChannelRepository)
        {
            this.messageRepository = messageRepository;
            this.queueItemRepository = queueItemRepository;
            this.entityRepository = entityRepository;
            this.attributeRepository = attributeRepository;
        }

        public override string Id => "34YdnRBnob9aBiHMYY1t";

        public override string Name => "Success Item Reporter";

        public override async Task Queue()
        {
            await Task.Run(() =>
            {
                var showDebugInfo = Options.FirstOrDefault(o => o.Name == "show_debug_info").Value;
                var showProgressInfo = Options.FirstOrDefault(o => o.Name == "show_progress_info").Value;
                var limit = 500;
                var offset = 0;
                while (true)
                {
                    // Create success message (MessageType = Information)
                    var successItems = queueItemRepository.GetQueuedItemsByStatus(QueueItemState.Success);
                    foreach (var item in successItems)
                    {
                        var messageText = string.Empty;
                        IIndexModel indexModel = null;
                        IndexItemModel itemModel = null; // indexer_report_alias_name, indexer_report_main_column_name
                        IEnumerable<OptionModel> options = null;
                        if (item.TargetEntityType == EntityType.Entity)
                        {
                            indexModel = entityRepository.GetById(item.TargetEntityId.ToString());
                            options = entityRepository.LoadOptions(indexModel.Id.ToString(), new List<string> { "Indexer" });
                            itemModel = entityRepository.GetIndexedItemById(indexModel, item.TargetItemId.ToString());
                        }
                        else
                        {
                            indexModel = attributeRepository.GetById(item.TargetEntityId.ToString());
                            options = attributeRepository.LoadOptions(indexModel.Id.ToString(), new List<string> { "Indexer" });
                            itemModel = attributeRepository.GetIndexedItemById(indexModel, item.TargetItemId.ToString());
                        }
                        var columnAliases = Regex.Split(options.FirstOrDefault(o => o.Key == "indexer_report_alias_name").Value ?? string.Empty, 
                            "[,;|]", 
                            RegexOptions.Multiline | RegexOptions.IgnoreCase);
                        var columnNames = Regex.Split(options.FirstOrDefault(o => o.Key == "indexer_report_main_column_name").Value ?? string.Empty, 
                            "[,;|]", 
                            RegexOptions.Multiline | RegexOptions.IgnoreCase);
                        var valueMessages = columnAliases.Select((c, i) => $@"_{c}_: {itemModel.GetValue(columnNames[i])?.ToString() ?? "(empty)"}");
                        messageText = $@"*{indexModel.Name}* ({string.Join(", ", valueMessages)}):";
                        if (showDebugInfo == bool.TrueString)
                        {
                            //var message = messageRepository.GetById(item.MessageId.ToString());
                            messageText = $@"{messageText}
* Index Info:
```{JsonConvert.SerializeObject(indexModel, Formatting.Indented)}```
* Index Item Info:
```{JsonConvert.SerializeObject(itemModel, Formatting.Indented)}```";
                        }

                        if (showProgressInfo == bool.TrueString)
                        {
                            var message = messageRepository.GetById(item.MessageId.ToString());
                            messageText = $@"{messageText}
* Progress:
```{message.Message}```";
                        }

                        var messageId = messageRepository.Create(new
                        {
                            CreatedAt = DateTime.Now.ToUnixTimestamp(),
                            Message = messageText,
                            MessageType = MessageType.Information,
                            Status = MessageStatus.None
                        });

                        messageRepository.LinkToReporter(messageId, ReporterModel);
                    }
                    offset += limit;
                }
            });
        }
    }
}
