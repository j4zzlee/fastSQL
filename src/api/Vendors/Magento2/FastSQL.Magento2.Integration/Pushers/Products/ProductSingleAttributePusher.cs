using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Processors;
using FastSQL.Sync.Core.Pusher;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Magento2.Integration.Pushers.Products
{
    public class ProductSingleAttributePusher : BaseAttributePusher
    {
        public ProductSingleAttributePusher(
            ProductSingleAttributePusherOptionManager optionManager,
            ProductProcessor entityProcessor, 
            SingleAttributeProcessor attributeProcessor, 
            FastProvider provider, 
            FastAdapter adapter,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            ConnectionRepository connectionRepository) : base(optionManager, entityProcessor, attributeProcessor, provider, adapter, entityRepository, attributeRepository, connectionRepository)
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

        public override string Remove(string destinationId = null)
        {
            throw new NotImplementedException();
        }

        public override string Update(string destinationId = null)
        {
            throw new NotImplementedException();
        }
    }
}
