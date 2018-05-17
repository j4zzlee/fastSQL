using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Indexer;
using FastSQL.Sync.Core.Processors;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Magento2.Integration.Indexers
{
    public class OrderItemIndexer : BaseEntityIndexer
    {
        public OrderItemIndexer(
            OrderItemProcessor processor, 
            OrderItemIndexerOptionManager optionManager,
            FastProvider provider,
            FastAdapter adapter,
            EntityRepository entityRepository,
            ConnectionRepository connectionRepository) : base(processor, optionManager, provider, adapter, entityRepository, connectionRepository)
        {
        }
    }
}
