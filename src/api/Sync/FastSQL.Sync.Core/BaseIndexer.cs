using System;
using System.Collections.Generic;
using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;

namespace FastSQL.Sync.Core
{
    public abstract class BaseIndexer : IIndexer
    {
        protected readonly IOptionManager OptionManager;

        public BaseIndexer(IOptionManager optionManager)
        {
            OptionManager = optionManager;
        }

        public virtual IEnumerable<OptionItem> Options => OptionManager.Options;

        public virtual IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return OptionManager.GetOptionsTemplate();
        }

        public abstract bool Is(EntityType entityType);
        public abstract void Persist(IEnumerable<object> data = null);

        public IOptionManager SetOptions(IEnumerable<OptionItem> options)
        {
            return OptionManager.SetOptions(options);
        }
    }

    public abstract class BaseEntityIndexer : BaseIndexer, IEntityIndexer
    {
        protected readonly EntityRepository EntityRepository;
        protected EntityModel EntityModel;
        protected IProcessor Processor;

        public BaseEntityIndexer(
            IOptionManager optionManager,
            EntityRepository entityRepository) : base(optionManager)
        {
            EntityRepository = entityRepository;
        }
        
        public override bool Is(EntityType entityType)
        {
            return entityType == EntityType.Entity;
        }

        public virtual IEntityIndexer SetEntity(Guid entityId)
        {
            EntityModel = EntityRepository.GetById(entityId.ToString());
            return this;
        }
    }

    public abstract class BaseAttributeIndexer : BaseIndexer, IAttributeIndexer
    {
        private readonly IProcessor attributeProcessor;
        private readonly IProcessor entityProcessor;
        protected readonly EntityRepository EntityRepository;
        protected readonly AttributeRepository AttributeRepository;
        protected EntityModel EntityModel;
        protected AttributeModel AttributeModel;

        public BaseAttributeIndexer(
            IOptionManager optionManager,
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
    }
}
