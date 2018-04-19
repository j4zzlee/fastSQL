using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Processors;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Magento2.Integration
{
    public class ProductPusher : BaseEntityPusher
    {
        public ProductPusher(ProductPusherOptionManager optionManager,
            ProductProcessor processor,
            FastProvider provider,
            EntityRepository entityRepository) : base(optionManager, processor, provider, entityRepository)
        {
        }

        public override void Push(Guid itemId)
        {
            throw new NotImplementedException();
        }
    }
}
