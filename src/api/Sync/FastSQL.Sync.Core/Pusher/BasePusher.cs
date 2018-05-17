using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;

namespace FastSQL.Sync.Core.Pusher
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

        public IPusher OnReport(Action<string> reporter)
        {
            _reporter = reporter;
            return this;
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
        public abstract IPusher SetIndex(IIndexModel model);
    }

    public abstract class BaseEntityPusher : BasePusher, IEntityPusher
    {
        protected readonly IProcessor Processor;
        protected readonly IRichProvider Provider;
        protected readonly IRichAdapter Adapter;
        protected readonly EntityRepository EntityRepository;
        protected readonly ConnectionRepository ConnectionRepository;
        protected EntityModel EntityModel;
        protected ConnectionModel ConnectionModel;

        public BaseEntityPusher(
            IOptionManager optionManager,
            IProcessor processor,
            IRichProvider provider,
            IRichAdapter adapter,
            EntityRepository entityRepository,
            ConnectionRepository connectionRepository) : base(optionManager)
        {
            Processor = processor;
            Provider = provider;
            Adapter = adapter;
            EntityRepository = entityRepository;
            ConnectionRepository = connectionRepository;
        }
        
        public bool IsImplemented(string processorId, string providerId)
        {
            return Processor.Id == processorId && Provider.Id == providerId;
        }

        public override IPusher SetIndex(IIndexModel model)
        {
            EntityModel = model as EntityModel;
            SpreadOptions();
            return this;
        }

        protected virtual IPusher SpreadOptions()
        {
            ConnectionModel = ConnectionRepository.GetById(EntityModel.SourceConnectionId.ToString());
            var connectionOptions = ConnectionRepository.LoadOptions(ConnectionModel.Id.ToString());
            var connectionOptionItems = connectionOptions.Select(c => new OptionItem { Name = c.Key, Value = c.Value });
            Adapter.SetOptions(connectionOptionItems);
            Provider.SetOptions(connectionOptionItems);
            return this;
        }
    }

    public abstract class BaseAttributePusher : BasePusher, IAttributePusher
    {
        protected readonly IProcessor EntityProcessor;
        protected readonly IProcessor AttributeProcessor;
        protected readonly IRichProvider Provider;
        protected readonly IRichAdapter Adapter;
        protected readonly EntityRepository EntityRepository;
        protected readonly AttributeRepository AttributeRepository;
        protected readonly ConnectionRepository ConnectionRepository;
        protected EntityModel EntityModel;
        protected AttributeModel AttributeModel;
        protected ConnectionModel ConnectionModel;

        public BaseAttributePusher(
            IOptionManager optionManager,
            IProcessor entityProcessor,
            IProcessor attributeProcessor, 
            IRichProvider provider,
            IRichAdapter adapter,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            ConnectionRepository connectionRepository) : base(optionManager)
        {
            this.EntityProcessor = entityProcessor;
            this.AttributeProcessor = attributeProcessor;
            this.Provider = provider;
            this.Adapter = adapter;
            this.EntityRepository = entityRepository;
            this.AttributeRepository = attributeRepository;
            this.ConnectionRepository = connectionRepository;
        }
        
        public bool IsImplemented(string attributeProcessorId, string entityProcessorId, string providerId)
        {
            return AttributeProcessor.Id == attributeProcessorId && EntityProcessor.Id == entityProcessorId && Provider.Id == providerId;
        }

        public override IPusher SetIndex(IIndexModel model)
        {
            AttributeModel = model as AttributeModel;
            EntityModel = EntityRepository.GetById(AttributeModel.EntityId.ToString());
            SpreadOptions();
            return this;
        }

        protected virtual IPusher SpreadOptions()
        {
            ConnectionModel = ConnectionRepository.GetById(AttributeModel.SourceConnectionId.ToString());
            var connectionOptions = ConnectionRepository.LoadOptions(ConnectionModel.Id.ToString());
            var connectionOptionItems = connectionOptions.Select(c => new OptionItem { Name = c.Key, Value = c.Value });
            Adapter.SetOptions(connectionOptionItems);
            Provider.SetOptions(connectionOptionItems);
            return this;
        }
    }
}
