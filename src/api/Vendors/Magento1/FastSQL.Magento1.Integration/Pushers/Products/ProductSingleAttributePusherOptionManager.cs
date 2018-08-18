﻿using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Magento1.Integration.Pushers.Products
{
    public class ProductSingleAttributePusherOptionManager : BaseOptionManager
    {
        public override IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return new List<OptionItem> {
                new OptionItem
                {
                    Name = "store_ids",
                    DisplayName = "Store ID",
                    Value = "0"
                },
            };
        }
    }
}
