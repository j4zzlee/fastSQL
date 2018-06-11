using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using Slack.Webhooks;

namespace FastSQL.Sync.Core.MessageDeliveryChannels.SlackChannel
{
    public class SlackMessageDeliverChannel : BaseMessageDeliverChannel
    {
        public SlackMessageDeliverChannel(SlackMessageDeliverChannelOptionManager optionManager) : base(optionManager)
        {
        }

        public override string Id => "paiQLvyPoxWX6W4BzGy6";

        public override string Name => "Slack WebHook";

        public override async Task DeliverMessage(string message, MessageType messageType)
        {
            await Task.Run(() =>
            {
                var slackClient = new SlackClient(Options.FirstOrDefault(o => o.Name == "url").Value);
                var icon = Options.FirstOrDefault(o => o.Name == "icon").Value;
                var canParse = Enum.TryParse(icon, out Emoji emoji);
                var slackMessage = new SlackMessage
                {
                    Channel = Options.FirstOrDefault(o => o.Name == "channel").Value,
                    Text = message,
                    IconEmoji = canParse ? Emoji.Accept : emoji,
                    Username = Options.FirstOrDefault(o => o.Name == "username").Value
                };
                slackClient.Post(slackMessage);
                Thread.Sleep(2000);
            });
        }
    }
}
