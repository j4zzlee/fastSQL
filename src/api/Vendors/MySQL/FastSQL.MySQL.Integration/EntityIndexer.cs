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
    public class EntityIndexer : BaseEntityIndexer
    {
        public EntityIndexer(EntityProcessor processor,
            FastProvider provider,
            EntityIndexerOptionManager optionManager,
            EntityRepository entityRepository) : base(processor, provider, optionManager, entityRepository)
        {
        }
    }
}
