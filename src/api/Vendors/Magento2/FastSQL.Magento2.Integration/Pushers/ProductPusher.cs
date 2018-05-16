using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Processors;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Magento2.Integration.Pushers
{
    public class ProductPusher : BaseEntityPusher
    {
        public ProductPusher(ProductPusherOptionManager optionManager,
            ProductProcessor processor,
            FastProvider provider,
            EntityRepository entityRepository) : base(optionManager, processor, provider, entityRepository)
        {
        }

        public override string Create()
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
