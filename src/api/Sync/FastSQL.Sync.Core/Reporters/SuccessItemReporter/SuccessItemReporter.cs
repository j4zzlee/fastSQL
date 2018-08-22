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
        public SuccessItemReporter(SuccessItemReporterOptionManager optionManager) : base(optionManager)
        {
        }

        public override string Id => "34YdnRBnob9aBiHMYY1t";

        public override string Name => "Success Item Reporter";

        public override async Task Queue()
        {
            await Task.Run(() =>
            {
                using (var messageRepository = RepositoryFactory.Create<MessageRepository>(this))
                using (var queueItemRepository = RepositoryFactory.Create<QueueItemRepository>(this))
                {
                    var showDebugInfo = Options.FirstOrDefault(o => o.Name == "show_debug_info").Value;
                    var showProgressInfo = Options.FirstOrDefault(o => o.Name == "show_progress_info").Value;
                    var limit = 500;
                    var offset = 0;
                    while (true)
                    {
                        // Create success message (MessageType = Information)
                        var successItems = queueItemRepository.GetQueuedItemsByStatus(
                            PushState.Success,
                            PushState.Failed | PushState.UnexpectedError | PushState.ValidationFailed | PushState.Ignore | PushState.Reported, // exclude items that are reported, failed...
                            limit,
                            offset);

                        foreach (var item in successItems)
                        {
                            var messageText = string.Empty;
                            messageText = item.TargetEntityType == EntityType.Entity
                                ? GetEntityMessage(item, out IIndexModel indexModel, out IndexItemModel itemModel)
                                : GetAttributeMessage(item, out indexModel, out itemModel);

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
                }
            });
        }

        private string GetAttributeMessage(QueueItemModel item, out IIndexModel indexModel, out IndexItemModel itemModel)
        {
            using (var entityRepository = RepositoryFactory.Create<EntityRepository>(this))
            using (var attributeRepository = RepositoryFactory.Create<AttributeRepository>(this))
            {
                var attributeModel = attributeRepository.GetById(item.TargetEntityId.ToString());
                var entityModel = entityRepository.GetById(attributeModel.EntityId.ToString());

                var entityOptions = entityRepository.LoadOptions(entityModel.Id.ToString(), new List<string> { "Indexer" });
                var attributeOptions = attributeRepository.LoadOptions(attributeModel.Id.ToString(), new List<string> { "Indexer" });

                var attributeItemModel = attributeRepository.GetIndexedItemById(attributeModel, item.TargetItemId);
                var entityItemModel = entityRepository.GetIndexedItemBySourceId(entityModel, attributeItemModel.GetSourceId());
                itemModel = attributeItemModel;

                var entityReporterMappingOption = entityOptions.FirstOrDefault(o => o.Key == "indexer_reporter_columns");
                var entityReporterMappingColumns = !string.IsNullOrWhiteSpace(entityReporterMappingOption?.Value)
                    ? JsonConvert.DeserializeObject<List<ReporterColumnMapping>>(entityReporterMappingOption.Value)
                    : new List<ReporterColumnMapping> { new ReporterColumnMapping {
                                    SourceName = "Value",
                                    MappingName = "Name",
                                    Key = true,
                                    Value = true
                                }};

                var attributeReporterMappingOption = attributeOptions.FirstOrDefault(o => o.Key == "indexer_reporter_columns");
                var attributeReporterMappingColumns = !string.IsNullOrWhiteSpace(attributeReporterMappingOption?.Value)
                    ? JsonConvert.DeserializeObject<List<ReporterColumnMapping>>(attributeReporterMappingOption.Value)
                    : new List<ReporterColumnMapping> { new ReporterColumnMapping {
                                    SourceName = "Value",
                                    MappingName = "Name",
                                    Key = true,
                                    Value = true
                                }};
                var keys = entityReporterMappingColumns.Where(r => r.Key)
                    .Select(r => $@"_{r.MappingName}_: {entityItemModel.GetValue(r.SourceName)?.ToString() ?? "(empty)"}");
                var vals = attributeReporterMappingColumns.Where(r => r.Value)
                    .Select(r => $@"_{r.MappingName}_: {attributeItemModel.GetValue(r.SourceName)?.ToString() ?? "(empty)"}");
                if (attributeReporterMappingColumns.Count == 1)
                {
                    vals = attributeReporterMappingColumns.Where(r => r.Value)
                    .Select(r => $@"{attributeItemModel.GetValue(r.SourceName)?.ToString() ?? "(empty)"}");
                }
                var executed = item.ExecutedAt.UnixTimeToTime().ToString("G");
                var executedIn = item.ExecutedAt - item.ExecuteAt;
                indexModel = attributeModel;
                return $@"*{indexModel.Name}* ({string.Join(", ", keys)}): {vals} {executed} in {executedIn} second(s)";
            }
        }

        private string GetEntityMessage(QueueItemModel item, out IIndexModel indexModel, out IndexItemModel itemModel)
        {
            using (var entityRepository = RepositoryFactory.Create<EntityRepository>(this))
            {
                indexModel = entityRepository.GetById(item.TargetEntityId.ToString());
                var options = entityRepository.LoadOptions(indexModel.Id.ToString(), new List<string> { "Indexer" });
                var reporterMappingColumnOption = options.FirstOrDefault(o => o.Key == "indexer_reporter_columns");
                var reporterMappingColumns = !string.IsNullOrWhiteSpace(reporterMappingColumnOption?.Value)
                    ? JsonConvert.DeserializeObject<List<ReporterColumnMapping>>(reporterMappingColumnOption.Value)
                    : new List<ReporterColumnMapping> { new ReporterColumnMapping {
                                    SourceName = "Value",
                                    MappingName = "Name",
                                    Key = true,
                                    Value = true
                                }};
                var entityModel = entityRepository.GetIndexedItemById(indexModel, item.TargetItemId);
                itemModel = entityModel;
                var keys = reporterMappingColumns.Where(r => r.Key)
                    .Select(r => $@"_{r.MappingName}_: {entityModel.GetValue(r.SourceName)?.ToString() ?? "(empty)"}");
                var vals = reporterMappingColumns.Where(r => r.Value)
                    .Select(r => $@"_{r.MappingName}_: {entityModel.GetValue(r.SourceName)?.ToString() ?? "(empty)"}");
                if (reporterMappingColumns.Count == 1)
                {
                    vals = reporterMappingColumns.Where(r => r.Value)
                    .Select(r => $@"{entityModel.GetValue(r.SourceName)?.ToString() ?? "(empty)"}");
                }
                var executed = item.ExecutedAt.UnixTimeToTime().ToString("G");
                var executedIn = item.ExecutedAt - item.ExecuteAt;

                return $@"*{indexModel.Name}* ({string.Join(", ", keys)}): {vals} {executed} in {executedIn} second(s)";
            }
        }
    }
}
