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
    public class CustomerIndexer : BaseEntityIndexer
    {
        public CustomerIndexer(
            CustomerProcessor processor, 
            CustomerIndexerOptionManager optionManager,
            FastAdapter adapter) : base(processor, optionManager, adapter)
        {
        }
    }
}
