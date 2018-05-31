using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastSQL.Sync.Core.Indexer
{
    public abstract class BaseEntityIndexer : BaseIndexer, IEntityIndexer
    {
        protected readonly IProcessor Processor;
        protected readonly EntityRepository EntityRepository;
        protected EntityModel EntityModel;

        public BaseEntityIndexer(
            IProcessor processor,
            IOptionManager optionManager,
            IRichProvider provider,
            IRichAdapter adapter,
            EntityRepository entityRepository,
            ConnectionRepository connectionRepository) : base(optionManager, adapter, provider, connectionRepository)
        {
            Processor = processor;
            EntityRepository = entityRepository;
        }

        public bool IsImplemented(string processorId, string providerId)
        {
            return Processor.Id == processorId && Provider.Id == providerId;
        }

        public override IIndexer SetIndex(IIndexModel model)
        {
            EntityModel = model as EntityModel;
            SpreadOptions();
            return this;
        }

        protected override IIndexModel GetIndexModel()
        {
            return EntityModel;
        }
        
        protected override BaseRepository GetRepository()
        {
            return EntityRepository;
        }
    }
}
