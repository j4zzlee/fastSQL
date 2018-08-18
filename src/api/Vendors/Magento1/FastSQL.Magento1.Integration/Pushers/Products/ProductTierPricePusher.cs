using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Processors;
using FastSQL.Sync.Core.Pusher;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Magento1.Integration.Pushers.Products
{
    public class ProductTierPricePusher : BaseAttributePusher
    {
        public ProductTierPricePusher(ProductTierPricePusherOptionManager optionManager,
            ProductProcessor entityProcessor, 
            TierPriceAttributeProcessor attributeProcessor,
            FastProvider provider,
            FastAdapter adapter) : base(optionManager, entityProcessor, attributeProcessor, provider, adapter)
        {
        }

        public override PushState Create(out string destinationId)
        {
            throw new NotImplementedException();
        }

        public override string GetDestinationId()
        {
            throw new NotImplementedException();
        }

        public override PushState Remove(string destinationId = null)
        {
            throw new NotImplementedException();
        }

        public override PushState Update(string destinationId = null)
        {
            throw new NotImplementedException();
        }
    }
}
