using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Magento1.Integration.Pushers
{
    public class ParentCategoryPusherOptionManager : BaseOptionManager
    {
        public override IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return new List<OptionItem> {
                new OptionItem
                {
                    Name = "root_category_id",
                    DisplayName = "Root Category Id"
                },
                new OptionItem
                {
                    Name = "is_anchor",
                    DisplayName = "Is Anchor",
                    Type = OptionType.Boolean
                },
                new OptionItem
                {
                    Name = "website_ids",
                    DisplayName = "Website Ids",
                    Value = "1"
                },
                new OptionItem
                {
                    Name = "store_id",
                    DisplayName = "Store Ids",
                    Value = "0"
                }
            };
        }
    }
}
