using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using System;

namespace FastSQL.Sync.Core
{
    public abstract class BaseAttributePuller : BasePuller, IAttributePuller
    {
        protected readonly EntityRepository EntityRepository;
        protected readonly AttributeRepository AttributeRepository;
        protected EntityModel EntityModel;
        protected AttributeModel AttributeModel;
        protected readonly IProcessor EntityProcessor;

        public BaseAttributePuller(
            IOptionManager optionManager,
            IProcessor entityProcessor,
            IProcessor attributeProcessor,
            IRichProvider provider,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository) : base(optionManager, attributeProcessor, provider)
        {
            EntityProcessor = entityProcessor;
            EntityRepository = entityRepository;
            AttributeRepository = attributeRepository;
        }

        public IProcessor GetEntityProcessor()
        {
            return EntityProcessor;
        }

        public bool IsEntityProcessor(string id)
        {
            return EntityProcessor.Id == id;
        }

        public bool IsEntityProcessor(IProcessor processor)
        {
            return EntityProcessor.Id == processor.Id;
        }

        public IAttributePuller SetAttribute(Guid attributeId)
        {
            AttributeModel = AttributeRepository.GetById(attributeId.ToString());
            EntityModel = EntityRepository.GetById(AttributeModel.EntityId.ToString());
            return this;
        }
    }
}
