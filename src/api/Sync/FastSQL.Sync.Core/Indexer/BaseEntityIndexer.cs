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
        protected EntityModel EntityModel;

        public BaseEntityIndexer(
            IProcessor processor,
            IOptionManager optionManager,
            IRichAdapter adapter) : base(optionManager, adapter)
        {
            Processor = processor;
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
            return RepositoryFactory.Create<EntityRepository>(this);
        }
    }
}
