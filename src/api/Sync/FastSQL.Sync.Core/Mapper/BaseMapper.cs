using System;
using System.Collections.Generic;
using System.Linq;
using FastSQL.Core;
using FastSQL.Sync.Core.Mapper;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;

namespace FastSQL.Sync.Core.Mapper
{
    public abstract class BaseMapper : IMapper
    {
        protected readonly IProcessor Processor;
        protected readonly IOptionManager OptionManager;
        protected readonly IRichProvider Provider;
        protected readonly IRichAdapter Adapter;
        protected readonly EntityRepository EntityRepository;
        protected readonly ConnectionRepository ConnectionRepository;
        protected EntityModel EntityModel;
        protected ConnectionModel ConnectionModel;
        protected Action<string> _reporter;

        public BaseMapper(
            IProcessor processor,
            IOptionManager optionManager, 
            IRichProvider provider,
            IRichAdapter adapter,
            EntityRepository entityRepository,
            ConnectionRepository connectionRepository)
        {
            Processor = processor;
            OptionManager = optionManager;
            Provider = provider;
            Adapter = adapter;
            EntityRepository = entityRepository;
            ConnectionRepository = connectionRepository;
        }

        public virtual IEnumerable<OptionItem> Options => OptionManager.Options;
        
        public virtual IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return OptionManager.GetOptionsTemplate();
        }
        
        public IMapper OnReport(Action<string> reporter)
        {
            _reporter = reporter;
            return this;
        }

        public IMapper Report(string message)
        {
            _reporter?.Invoke(message);
            return this;
        }
        
        public virtual IOptionManager SetOptions(IEnumerable<OptionItem> options)
        {
            return OptionManager.SetOptions(options);
        }
        
        public bool IsImplemented(string processorId, string providerId)
        {
            return Processor.Id == processorId && Provider.Id == providerId;
        }

        public IMapper SetIndex(IIndexModel model)
        {
            EntityModel = model as EntityModel;
            SpreadOptions();
            return this;
        }

        protected virtual IMapper SpreadOptions()
        {
            ConnectionModel = ConnectionRepository.GetById(EntityModel.SourceConnectionId.ToString());
            var connectionOptions = ConnectionRepository.LoadOptions(ConnectionModel.Id.ToString());
            var connectionOptionItems = connectionOptions.Select(c => new OptionItem { Name = c.Key, Value = c.Value });
            Adapter.SetOptions(connectionOptionItems);
            Provider.SetOptions(connectionOptionItems);
            return this;
        }

        public abstract MapResult Map(object lastToken = null);
    }
}
