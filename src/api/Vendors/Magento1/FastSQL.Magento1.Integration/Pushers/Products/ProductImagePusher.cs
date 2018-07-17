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
    public class ProductImagePusher : BaseAttributePusher
    {
        public ProductImagePusher(ProductImagePusherOptionManager optionManager,
            ProductProcessor entityProcessor,
            ImageAttributeProcessor attributeProcessor,
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
