using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Indexer
{
    public class AttributeIndexer : BaseIndexer, IAttributeIndexer
    {
        private readonly IProcessor attributeProcessor;
        private readonly IProcessor entityProcessor;
        protected readonly EntityRepository EntityRepository;
        protected readonly AttributeRepository AttributeRepository;
        protected EntityModel EntityModel;
        protected AttributeModel AttributeModel;

        public AttributeIndexer(
            AttributeIndexerOptionManager optionManager,
            IProcessor attributeProcessor,
            IProcessor entityProcessor,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository) : base(optionManager)
        {
            this.attributeProcessor = attributeProcessor;
            this.entityProcessor = entityProcessor;
            EntityRepository = entityRepository;
            AttributeRepository = attributeRepository;
        }

        public virtual IAttributeIndexer SetAttribute(Guid attributeId)
        {
            AttributeModel = AttributeRepository.GetById(attributeId.ToString());
            EntityModel = EntityRepository.GetById(AttributeModel.EntityId.ToString());
            return this;
        }

        public override bool Is(EntityType entityType)
        {
            return entityType == EntityType.Attribute;
        }

        public override void Persist(IEnumerable<object> data = null, bool lastPage = false)
        {
            throw new NotImplementedException();
        }
    }
}
