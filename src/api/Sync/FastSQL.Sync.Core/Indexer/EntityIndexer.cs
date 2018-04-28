using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Indexer
{
    public class EntityIndexer : BaseIndexer, IEntityIndexer
    {
        protected readonly EntityRepository EntityRepository;
        protected EntityModel EntityModel;
        protected IProcessor Processor;

        public EntityIndexer(
            EntityIndexerOptionManager optionManager,
            EntityRepository entityRepository) : base(optionManager)
        {
            EntityRepository = entityRepository;
        }

        public override bool Is(EntityType entityType)
        {
            return entityType == EntityType.Entity;
        }

        public override void Persist(IEnumerable<object> data = null, bool lastPage = false)
        {
            throw new NotImplementedException();
        }

        public virtual IEntityIndexer SetEntity(Guid entityId)
        {
            EntityModel = EntityRepository.GetById(entityId.ToString());
            return this;
        }
    }
}
