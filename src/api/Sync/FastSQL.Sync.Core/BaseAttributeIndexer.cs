using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using System;

namespace FastSQL.Sync.Core
{
    public abstract class BaseAttributeIndexer: BaseIndexer, IAttributeIndexer
    {
        protected readonly EntityRepository EntityRepository;
        protected readonly AttributeRepository AttributeRepository;
        protected EntityModel EntityModel;
        protected AttributeModel AttributeModel;

        public BaseAttributeIndexer(
            IOptionManager optionManager,
            IProcessor processor,
            IRichProvider provider,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository) : base(optionManager, processor, provider)
        {
            EntityRepository = entityRepository;
            AttributeRepository = attributeRepository;
        }

        public IAttributeIndexer SetAttribute(Guid attributeId)
        {
            AttributeModel = AttributeRepository.GetById(attributeId.ToString());
            EntityModel = EntityRepository.GetById(AttributeModel.EntityId.ToString());
            return this;
        }
    }
}
