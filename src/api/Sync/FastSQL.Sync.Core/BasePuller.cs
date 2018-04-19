using System;
using System.Collections.Generic;
using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;

namespace FastSQL.Sync.Core
{
    public abstract class BasePuller : IPuller
    {
        protected readonly IOptionManager OptionManager;
        protected readonly IRichProvider Provider;

        public BasePuller(IOptionManager optionManager, IRichProvider provider)
        {
            OptionManager = optionManager;
            Provider = provider;
        }

        public virtual IEnumerable<OptionItem> Options => OptionManager.Options;
        
        public virtual IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return OptionManager.GetOptionsTemplate();
        }
        
        public IRichProvider GetProvider()
        {
            return Provider;
        }
        
        public abstract PullResult PullNext(object lastToken = null);

        public virtual IOptionManager SetOptions(IEnumerable<OptionItem> options)
        {
            return OptionManager.SetOptions(options);
        }
    }

    public abstract class BaseEntityPuller : BasePuller, IEntityPuller
    {
        protected readonly IProcessor EntityProcessor;
        protected readonly EntityRepository EntityRepository;
        protected EntityModel EntityModel;
        public BaseEntityPuller(IOptionManager optionManager,
            IProcessor processor,
            IRichProvider provider,
            EntityRepository entityRepository) : base(optionManager, provider)
        {
            EntityProcessor = processor;
            EntityRepository = entityRepository;
        }

        public IProcessor GetProcessor()
        {
            return EntityProcessor;
        }

        public bool IsImplemented(string processorId, string providerId)
        {
            return EntityProcessor.Id == processorId && Provider.Id == providerId;
        }

        public IEntityPuller SetEntity(Guid entityId)
        {
            EntityModel = EntityRepository.GetById(entityId.ToString());
            return this;
        }
    }

    public abstract class BaseAttributePuller : BasePuller, IAttributePuller
    {
        protected readonly EntityRepository EntityRepository;
        protected readonly AttributeRepository AttributeRepository;
        protected EntityModel EntityModel;
        protected AttributeModel AttributeModel;
        protected readonly IProcessor EntityProcessor;
        protected readonly IProcessor AttributeProcessor;

        public BaseAttributePuller(
            IOptionManager optionManager,
            IProcessor entityProcessor,
            IProcessor attributeProcessor,
            IRichProvider provider,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository) : base(optionManager, provider)
        {
            EntityProcessor = entityProcessor;
            AttributeProcessor = attributeProcessor;
            EntityRepository = entityRepository;
            AttributeRepository = attributeRepository;
        }

        public virtual IProcessor GetEntityProcessor()
        {
            return EntityProcessor;
        }

        public virtual IAttributePuller SetAttribute(Guid attributeId)
        {
            AttributeModel = AttributeRepository.GetById(attributeId.ToString());
            EntityModel = EntityRepository.GetById(AttributeModel.EntityId.ToString());
            return this;
        }

        public virtual IProcessor GetAttributeProcessor()
        {
            return AttributeProcessor;
        }

        public bool IsImplemented(string attributeProcessorId, string entityProcessorId, string providerId)
        {
            return Provider.Id == providerId && AttributeProcessor.Id == attributeProcessorId && EntityProcessor.Id == entityProcessorId;
        }
    }
}
