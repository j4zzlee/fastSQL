using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Indexer
{
    public abstract class BaseEntityIndexer : BaseIndexer, IEntityIndexer
    {
        protected readonly IProcessor Processor;
        protected readonly IRichProvider Provider;
        protected readonly EntityRepository EntityRepository;
        protected EntityModel EntityModel;

        public BaseEntityIndexer(
            IProcessor processor,
            IRichProvider provider,
            IOptionManager optionManager,
            EntityRepository entityRepository) : base(optionManager)
        {
            this.Processor = processor;
            this.Provider = provider;
            EntityRepository = entityRepository;
        }

        public bool IsImplemented(string processorId, string providerId)
        {
            return Processor.Id == processorId && Provider.Id == providerId;
        }

        public virtual IEntityIndexer SetEntity(EntityModel entity)
        {
            EntityModel = entity;
            return this;
        }
        
        protected override IIndexModel GetIndexer()
        {
            return EntityModel;
        }
        
        protected override BaseRepository GetRepository()
        {
            return EntityRepository;
        }
    }
}
