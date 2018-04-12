using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using System;

namespace FastSQL.Sync.Core
{
    public abstract class BaseEntityPuller : BasePuller, IEntityPuller
    {
        protected readonly EntityRepository EntityRepository;
        protected EntityModel EntityModel;
        public BaseEntityPuller(IOptionManager optionManager,
            IProcessor processor,
            IRichProvider provider,
            EntityRepository entityRepository) : base(optionManager, processor, provider)
        {
            EntityRepository = entityRepository;
        }

        public IEntityPuller SetEntity(Guid entityId)
        {
            EntityModel = EntityRepository.GetById(entityId.ToString());
            return this;
        }
    }
}
