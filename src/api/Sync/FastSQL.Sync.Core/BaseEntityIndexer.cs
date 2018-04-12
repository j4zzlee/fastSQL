using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core
{
    public abstract class BaseEntityIndexer : BaseIndexer, IEntityIndexer
    {
        protected readonly EntityRepository EntityRepository;
        protected EntityModel EntityModel;

        public BaseEntityIndexer(
            IOptionManager optionManager,
            IProcessor processor,
            IRichProvider provider,
            EntityRepository entityRepository) : base(optionManager, processor, provider)
        {
            EntityRepository = entityRepository;
        }

        public IEntityIndexer SetEntity(Guid entityId)
        {
            EntityModel = EntityRepository.GetById(entityId.ToString());
            return this;
        }
    }
}
