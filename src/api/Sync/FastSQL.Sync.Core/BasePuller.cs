using System;
using System.Collections.Generic;
using System.Linq;
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
        protected readonly IRichAdapter Adapter;
        protected readonly EntityRepository EntityRepository;
        private readonly ConnectionRepository ConnectionRepository;
        protected EntityModel EntityModel;
        protected ConnectionModel ConnectionModel;
        public BaseEntityPuller(IOptionManager optionManager,
            IProcessor processor,
            IRichProvider provider,
            IRichAdapter adapter,
            EntityRepository entityRepository,
            ConnectionRepository connectionRepository) : base(optionManager, provider)
        {
            EntityProcessor = processor;
            Adapter = adapter;
            EntityRepository = entityRepository;
            ConnectionRepository = connectionRepository;
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
            SpreadOptions();
            return this;
        }

        public IEntityPuller SetEntity(EntityModel entity)
        {
            EntityModel = entity;
            SpreadOptions();
            return this;
        }

        protected virtual IEntityPuller SpreadOptions()
        {
            ConnectionModel = ConnectionRepository.GetById(EntityModel.SourceConnectionId.ToString());
            var connectionOptions = ConnectionRepository.LoadOptions(ConnectionModel.Id);
            var connectionOptionItems = connectionOptions.Select(c => new OptionItem { Name = c.Key, Value = c.Value });
            Adapter.SetOptions(connectionOptionItems);
            Provider.SetOptions(connectionOptionItems);
            return this;
        }
    }

    public abstract class BaseAttributePuller : BasePuller, IAttributePuller
    {
        protected readonly EntityRepository EntityRepository;
        protected readonly AttributeRepository AttributeRepository;
        protected readonly ConnectionRepository ConnectionRepository;
        protected EntityModel EntityModel;
        protected AttributeModel AttributeModel;
        protected ConnectionModel ConnectionModel;
        protected readonly IProcessor EntityProcessor;
        protected readonly IProcessor AttributeProcessor;
        protected readonly IRichAdapter Adapter;

        public BaseAttributePuller(
            IOptionManager optionManager,
            IProcessor entityProcessor,
            IProcessor attributeProcessor,
            IRichProvider provider,
            IRichAdapter adapter,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            ConnectionRepository connectionRepository) : base(optionManager, provider)
        {
            EntityProcessor = entityProcessor;
            AttributeProcessor = attributeProcessor;
            Adapter = adapter;
            EntityRepository = entityRepository;
            AttributeRepository = attributeRepository;
            this.ConnectionRepository = connectionRepository;
        }

        public virtual IProcessor GetEntityProcessor()
        {
            return EntityProcessor;
        }

        public virtual IAttributePuller SetAttribute(Guid attributeId)
        {
            AttributeModel = AttributeRepository.GetById(attributeId.ToString());
            EntityModel = EntityRepository.GetById(AttributeModel.EntityId.ToString());
            SpreadOptions();
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

        public IAttributePuller SetAttribute(AttributeModel attribute, EntityModel entityModel = null)
        {
            AttributeModel = attribute;
            EntityModel = entityModel;
            SpreadOptions();
            return this;
        }

        protected virtual IAttributePuller SpreadOptions()
        {
            ConnectionModel = ConnectionRepository.GetById(AttributeModel.SourceConnectionId.ToString());
            var connectionOptions = ConnectionRepository.LoadOptions(ConnectionModel.Id);
            var connectionOptionItems = connectionOptions.Select(c => new OptionItem { Name = c.Key, Value = c.Value });
            Adapter.SetOptions(connectionOptionItems);
            Provider.SetOptions(connectionOptionItems);
            return this;
        }
    }
}
