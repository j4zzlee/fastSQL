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
            FastProvider provider,
            FastAdapter adapter,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            ConnectionRepository connectionRepository) : base(entityProcessor, attributeProcessor, optionManager, provider, adapter, entityRepository, attributeRepository, connectionRepository)
        {
        }
    }
}
