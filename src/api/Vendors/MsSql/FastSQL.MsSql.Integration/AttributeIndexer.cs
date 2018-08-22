using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Indexer;
using FastSQL.Sync.Core.Processors;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.MsSql.Integration
{
    public class AttributeIndexer : BaseAttributeIndexer
    {
        public AttributeIndexer(
            EntityProcessor entityProcessor,
            AttributeProcessor attributeProcessor,
            AttributeIndexerOptionManager optionManager,
            FastAdapter adapter) : base(entityProcessor, attributeProcessor, optionManager, adapter)
        {
        }
    }
}
