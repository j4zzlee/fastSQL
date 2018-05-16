using System;
using System.Collections.Generic;
using System.Text;
using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;

namespace FastSQL.Sync.Core
{
    public abstract class BasePusher : IPusher
    {
        protected readonly IOptionManager OptionManager;
        protected Action<string> _reporter;
        protected IndexItemModel _item;

        public BasePusher(IOptionManager optionManager)
        {
            OptionManager = optionManager;
        }

        public virtual IEnumerable<OptionItem> Options => OptionManager.Options;
        
        public virtual IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return OptionManager.GetOptionsTemplate();
        }

        public void OnReport(Action<string> reporter)
        {
            _reporter = reporter;
        }
        
        public IPusher SetItem(IndexItemModel item)
        {
            _item = item;
            return this;
        }

        public IOptionManager SetOptions(IEnumerable<OptionItem> options)
        {
            return OptionManager.SetOptions(options);
        }

        public abstract string Create();
        public abstract string GetDestinationId();
        public abstract string Remove(string destinationId = null);
        public abstract string Update(string destinationId = null);
    }

    public abstract class BaseEntityPusher : BasePusher, IEntityPusher
    {
        public BaseEntityPusher(
            IOptionManager optionManager,
            IProcessor processor,
            IRichProvider provider,
            EntityRepository entityRepository) : base(optionManager)
        {
            Processor = processor;
            Provider = provider;
            EntityRepository = entityRepository;
        }

        protected IProcessor Processor;

        protected IRichProvider Provider;

        protected EntityRepository EntityRepository;
        protected EntityModel entityModel;
        protected object Item;

        public IProcessor GetProcessor()
        {
            return Processor;
        }

        public IRichProvider GetProvider()
        {
            return Provider;
        }

        public bool IsImplemented(string processorId, string providerId)
        {
            return Processor.Id == processorId && Provider.Id == providerId;
        }

        public IEntityPusher SetEntity(EntityModel entity)
        {
            entityModel = entity;
            return this;
        }
    }

    public abstract class BaseAttributePusher : BasePusher, IAttributePusher
    {
        protected readonly IProcessor EntityProcessor;
        protected readonly IProcessor AttributeProcessor;
        protected readonly IRichProvider Provider;
        protected readonly EntityRepository EntityRepository;
        protected readonly AttributeRepository AttributeRepository;
        protected EntityModel entityModel;
        protected AttributeModel attributeModel;
        protected object Item;

        public BaseAttributePusher(
            IOptionManager optionManager,
            IProcessor entityProcessor,
            IProcessor attributeProcessor, 
            IRichProvider provider,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository) : base(optionManager)
        {
            this.EntityProcessor = entityProcessor;
            this.AttributeProcessor = attributeProcessor;
            this.Provider = provider;
            this.EntityRepository = entityRepository;
            this.AttributeRepository = attributeRepository;
        }

        public IProcessor GetAttributeProcessor()
        {
            return AttributeProcessor;
        }

        public IProcessor GetEntityProcessor()
        {
            return EntityProcessor;
        }

        public IRichProvider GetProvider()
        {
            return Provider;
        }

        public bool IsImplemented(string attributeProcessorId, string entityProcessorId, string providerId)
        {
            return AttributeProcessor.Id == attributeProcessorId && EntityProcessor.Id == entityProcessorId && Provider.Id == providerId;
        }

        public IAttributePusher SetAttribute(AttributeModel attribute, EntityModel entity)
        {
            attributeModel = attribute;
            entityModel = entity;
            return this;
        }
    }
}
