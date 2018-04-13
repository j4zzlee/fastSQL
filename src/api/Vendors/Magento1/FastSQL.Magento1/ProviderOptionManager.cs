using FastSQL.Core;
using System;
using System.Collections.Generic;

namespace FastSQL.Magento1
{
    public class ProviderOptionManager : BaseOptionMananger
    {
        public override IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return new List<OptionItem>
            {
                new OptionItem
                {
                    Name = "api_uri",
                    DisplayName = "URI",
                    Type = OptionType.Text
                },
                new OptionItem
                {
                    Name = "api_key",
                    DisplayName = "Api Key",
                    Type = OptionType.Password
                },
                new OptionItem
                {
                    Name = "api_user",
                    DisplayName = "Api User",
                    Type = OptionType.Text
                }
            };
        }
    }
}
