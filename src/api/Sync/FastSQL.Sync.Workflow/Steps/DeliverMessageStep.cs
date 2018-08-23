using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.MessageDeliveryChannels;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Reporters;
using FastSQL.Sync.Core.Repositories;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace FastSQL.Sync.Workflow.Steps
{
    public class DeliverMessageStep : BaseStepBodyInvoker
    {
        private readonly IEnumerable<IReporter> reporters;
        private readonly IEnumerable<IMessageDeliveryChannel> channels;

        public DeliverMessageStep(
            IEnumerable<IReporter> reporters,
            IEnumerable<IMessageDeliveryChannel> channels) : base()
        {
            this.reporters = reporters;
            this.channels = channels;
        }

        public override async Task Invoke(IStepExecutionContext context = null)
        {
            var messageRepository = ResolverFactory.Resolve<MessageRepository>();
            var reporterRepository = ResolverFactory.Resolve<ReporterRepository>();
            var messageDeliveryChannelRepository = ResolverFactory.Resolve<MessageDeliveryChannelRepository>();
            var logger = ResolverFactory.Resolve<ILogger>("SyncService");
            var errorLogger = ResolverFactory.Resolve<ILogger>("Error");
            try
            {
                var undeliverMessages = messageRepository.GetUndeliveredMessages(100, 0);
                var linkedReportModels = messageRepository.GetLinkedReports(undeliverMessages.Select(m => m.Id));
                var reportModels = reporterRepository.GetByIds(linkedReportModels.Select(r => r.ReporterId.ToString()).Distinct());
                var linkedDeliveryChannels = reporterRepository.GetLinkedDeliveryChannels(reportModels.Select(r => r.Id.ToString()));
                var deliveryChannelModels = messageDeliveryChannelRepository.GetByIds(linkedDeliveryChannels.Select(c => c.DeliveryChannelId.ToString()).Distinct());
                // Order same same channels, these one should be run in sequence, e.g: slack cannot be send in parallel
                var channelDict = new Dictionary<string, List<MessageDeliveryChannelModel>>();
                foreach (var channel in deliveryChannelModels)
                {
                    if (!channelDict.ContainsKey(channel.ChannelId))
                    {
                        channelDict.Add(channel.ChannelId, new List<MessageDeliveryChannelModel> { });
                    }

                    channelDict[channel.ChannelId].Add(channel);
                }

                await Task.Run(() => Parallel.ForEach(channelDict, async (c, i) =>
                {
                    foreach (var channel in c.Value)
                    {
                        var relatedReports = reportModels.Where(rp => linkedDeliveryChannels.Any(dc => dc.DeliveryChannelId == channel.Id));
                        var relatedMessages = undeliverMessages.Where(m => linkedReportModels.Any(rm => rm.MessageId == m.Id && relatedReports.Any(r => r.Id == rm.ReporterId)));
                        var messageDic = new Dictionary<MessageType, List<MessageModel>>();
                        foreach (var relatedMessage in relatedMessages)
                        {
                            if (!messageDic.ContainsKey(relatedMessage.MessageType))
                            {
                                messageDic.Add(relatedMessage.MessageType, new List<MessageModel>());
                            }

                            messageDic[relatedMessage.MessageType].Add(relatedMessage);
                        }
                        var options = messageDeliveryChannelRepository.LoadOptions(channel.Id.ToString(), channel.EntityType);
                        var delieveryChannel = channels.FirstOrDefault(cc => cc.Id == channel.ChannelId);
                        delieveryChannel.SetOptions(options.Select(o => new OptionItem { Name = o.Key, Value = o.Value }));
                        delieveryChannel.OnReport(s => logger.Information(s));
                        foreach (var dict in messageDic)
                        {
                            if (dict.Value == null || dict.Value.Count == 0)
                            {
                                continue;
                            }
                            try
                            {
                                // send bulk messages in sequence because of MessageType is different
                                await delieveryChannel.DeliverMessage(string.Join("\n", dict.Value.Select(v => v.Message)), dict.Key);
                            }
                            finally
                            {
                                // Update the message as repotered no matter what
                                messageRepository.SetMessagesAsReported(dict.Value.Select(v => v.Id));
                            }
                        }
                    }
                }));
            }
            catch (Exception ex)
            {
                errorLogger.Error(ex, ex.Message);
                throw;
            }
            finally
            {
                ResolverFactory.Release(logger);
                ResolverFactory.Release(errorLogger);
                logger = null;
                errorLogger = null;
            }
        }
    }
}
