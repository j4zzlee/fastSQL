﻿using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Indexer;
using FastSQL.Sync.Core.Processors;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Magento2.Integration.Indexers
{
    public class OrderIndexer : BaseEntityIndexer
    {
        public OrderIndexer(
            OrderProcessor processor,
            OrderIndexerOptionManager optionManager,
            FastAdapter adapter) : base(processor, optionManager, adapter)
        {
        }
    }
}
