using System;
using System.Collections.Generic;
using System.Linq;
using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;

namespace FastSQL.Sync.Core.Puller
{
    public abstract class BasePuller : IPuller
    {
        protected readonly IOptionManager OptionManager;
        protected readonly IRichProvider Provider;
        private Action<string> _reporter;
        protected ConnectionModel ConnectionModel;

        public IRichAdapter Adapter { get; }
        protected ConnectionRepository ConnectionRepository { get; }

        public BasePuller(IOptionManager optionManager, IRichProvider provider, IRichAdapter adapter, ConnectionRepository connectionRepository)
        {
            OptionManager = optionManager;
            Provider = provider;
            Adapter = adapter;
            ConnectionRepository = connectionRepository;
        }

        public virtual IEnumerable<OptionItem> Options => OptionManager.Options;
        
        public virtual IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return OptionManager.GetOptionsTemplate();
        }
        
        public abstract IPuller Init();
        public abstract bool Initialized();
        public abstract IPuller SetIndex(IIndexModel model);
        public abstract IIndexModel GetIndexModel();

        public IPuller OnReport(Action<string> reporter)
        {
            _reporter = reporter;
            return this;
        }

        public abstract PullResult Preview();

        public abstract PullResult PullNext(object lastToken = null);

        public IPuller Report(string message)
        {
            _reporter?.Invoke(message);
            return this;
        }

        public virtual IOptionManager SetOptions(IEnumerable<OptionItem> options)
        {
            return OptionManager.SetOptions(options);
        }

        protected virtual IPuller SpreadOptions()
        {
            ConnectionModel = ConnectionRepository.GetById(GetIndexModel().SourceConnectionId.ToString());
            var connectionOptions = ConnectionRepository.LoadOptions(ConnectionModel.Id.ToString());
            var connectionOptionItems = connectionOptions.Select(c => new OptionItem { Name = c.Key, Value = c.Value });
            Adapter.SetOptions(connectionOptionItems);
            Provider.SetOptions(connectionOptionItems);
            return this;
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
            IRichAdapter adapter,
            EntityRepository entityRepository,
            ConnectionRepository connectionRepository) : base(optionManager, provider, adapter, connectionRepository)
        {
            EntityProcessor = processor;
            EntityRepository = entityRepository;
        }
        
        public bool IsImplemented(string processorId, string providerId)
        {
            return EntityProcessor.Id == processorId && Provider.Id == providerId;
        }
        
        public override IPuller SetIndex(IIndexModel model)
        {
            EntityModel = model as EntityModel;
            SpreadOptions();
            return this;
        }

        public override IIndexModel GetIndexModel()
        {
            return EntityModel;
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
            IRichAdapter adapter,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            ConnectionRepository connectionRepository) : base(optionManager, provider, adapter, connectionRepository)
        {
            EntityProcessor = entityProcessor;
            AttributeProcessor = attributeProcessor;
            EntityRepository = entityRepository;
            AttributeRepository = attributeRepository;
        }

        public override IPuller SetIndex(IIndexModel model)
        {
            AttributeModel = model as AttributeModel;
            EntityModel = EntityRepository.GetById(AttributeModel.EntityId.ToString());
            SpreadOptions();
            return this;
        }
        
        public bool IsImplemented(string attributeProcessorId, string entityProcessorId, string providerId)
        {
            return Provider.Id == providerId && AttributeProcessor.Id == attributeProcessorId && EntityProcessor.Id == entityProcessorId;
        }
        
        public override IIndexModel GetIndexModel()
        {
            return AttributeModel;
        }
    }
}
