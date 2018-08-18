using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Processors;
using FastSQL.Sync.Core.Pusher;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Magento2.Integration.Pushers
{
    public class ProductAttributeOptionPusher : BaseEntityPusher
    {
        public ProductAttributeOptionPusher(
            ProductAttributeOptionPusherOptionManager optionManager,
            AttributeOptionProcessor processor,
            FastProvider provider,
            FastAdapter adapter) : base(optionManager, processor, provider, adapter)
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
