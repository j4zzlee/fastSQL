using FastSQL.Core;
using System;
using System.Collections.Generic;

namespace FastSQL.Magento2
{
    public class ProviderOptionManager : BaseOptionManager
    {
        public override IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return new List<OptionItem>
            {
                new OptionItem
                {
                    Name = "api_uri",
                    DisplayName = "Uri",
                    Type = OptionType.Text
                },
                new OptionItem
                {
                    Name = "consumer_key",
                    DisplayName = "Consumer Key",
                    Type = OptionType.Text
                },
                new OptionItem
                {
                    Name = "consumer_secret",
                    DisplayName = "Consumer Secret",
                    Type = OptionType.Password
                },
                new OptionItem
                {
                    Name = "access_token",
                    DisplayName = "Access Token",
                    Type = OptionType.Text
                },
                new OptionItem
                {
                    Name = "access_token_secret",
                    DisplayName = "Access Token Secret",
                    Type = OptionType.Password
                }
            };
        }
    }
}
