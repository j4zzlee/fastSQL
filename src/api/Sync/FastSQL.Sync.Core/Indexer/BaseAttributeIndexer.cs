using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastSQL.Sync.Core.Indexer
{
    public class BaseAttributeIndexer : BaseIndexer, IAttributeIndexer
    {
        protected readonly IProcessor AttributeProcessor;
        protected readonly IProcessor EntityProcessor;
        protected readonly IRichProvider Provider;
        protected readonly IRichAdapter Adapter;
        protected readonly EntityRepository EntityRepository;
        protected readonly AttributeRepository AttributeRepository;
        protected readonly ConnectionRepository ConnectionRepository;
        protected EntityModel EntityModel;
        protected AttributeModel AttributeModel;
        protected ConnectionModel ConnectionModel;

        public BaseAttributeIndexer(
            IProcessor entityProcessor,
            IProcessor attributeProcessor,
            IOptionManager optionManager,
            IRichProvider provider,
            IRichAdapter adapter,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            ConnectionRepository connectionRepository) : base(optionManager)
        {
            AttributeProcessor = attributeProcessor;
            EntityProcessor = entityProcessor;
            Provider = provider;
            Adapter = adapter;
            EntityRepository = entityRepository;
            AttributeRepository = attributeRepository;
            ConnectionRepository = connectionRepository;
        }
        
        public override IIndexer SetIndex(IIndexModel model)
        {
            AttributeModel = model as AttributeModel;
            EntityModel = EntityRepository.GetById(AttributeModel.EntityId.ToString());
            SpreadOptions();
            return this;
        }

        protected override IIndexModel GetIndexer()
        {
            return AttributeModel;
        }
        
        protected override BaseRepository GetRepository()
        {
            return AttributeRepository;
        }

        public bool IsImplemented(string attributeProcessorId, string entityProcessorId, string providerId)
        {
            return AttributeProcessor.Id == attributeProcessorId && EntityProcessor.Id == entityProcessorId && Provider.Id == providerId;
        }
        
        protected virtual IIndexer SpreadOptions()
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
