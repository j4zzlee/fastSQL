using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Reporters
{
    public class ErrorReporterOptionManager : BaseOptionMananger
    {
        public override IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return new List<OptionItem>
            {
                //new OptionItem
                //{
                //    Name = "username",
                //    DisplayName = "Username",
                //    Description = @"Username",
                //    Value = string.Empty,
                //    Example = "#webhookuser",
                //    OptionGroupNames = new List<string>{ "SlackMessageDeliverChannel" },
                //},
                //new OptionItem
                //{
                //    Name = "channel",
                //    DisplayName = "Channel",
                //    Description = @"Channel",
                //    Value = string.Empty,
                //    Example = "#webhookchannel",
                //    OptionGroupNames = new List<string>{ "SlackMessageDeliverChannel" },
                //},
                //new OptionItem
                //{
                //    Name = "url",
                //    DisplayName = "Url",
                //    Description = @"Channel Url",
                //    Value = string.Empty,
                //    Example = "https://hooks.slack.com/services/<my-slack>",
                //    OptionGroupNames = new List<string>{ "SlackMessageDeliverChannel" },
                //},
                //new OptionItem
                //{
                //    Name = "icon",
                //    DisplayName = "Icon",
                //    Description = @"Icon",
                //    Value = string.Empty,
                //    Example = ":white_check_mark:",
                //    OptionGroupNames = new List<string>{ "SlackMessageDeliverChannel" },
                //},
            };
        }
    }
}
