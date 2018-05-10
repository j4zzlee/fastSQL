using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Indexer;
using FastSQL.Sync.Core.Processors;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.MySQL.Integration
{
    public class AttributeIndexer : BaseAttributeIndexer
    {
        public AttributeIndexer(
            AttributeProcessor attributeProcessor,
            EntityProcessor entityProcessor,
            FastProvider provider,
            AttributeIndexerOptionManager optionManager,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository) : base(attributeProcessor, entityProcessor, provider, optionManager, entityRepository, attributeRepository)
        {
        }
    }
}
