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
        protected readonly EntityRepository EntityRepository;
        protected readonly AttributeRepository AttributeRepository;
        protected EntityModel EntityModel;
        protected AttributeModel AttributeModel;

        public BaseAttributeIndexer(
            IProcessor entityProcessor,
            IProcessor attributeProcessor,
            IOptionManager optionManager,
            IRichProvider provider,
            IRichAdapter adapter,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            ConnectionRepository connectionRepository) : base(optionManager, adapter, provider, connectionRepository)
        {
            AttributeProcessor = attributeProcessor;
            EntityProcessor = entityProcessor;
            EntityRepository = entityRepository;
            AttributeRepository = attributeRepository;
        }
        
        public override IIndexer SetIndex(IIndexModel model)
        {
            AttributeModel = model as AttributeModel;
            EntityModel = EntityRepository.GetById(AttributeModel.EntityId.ToString());
            SpreadOptions();
            return this;
        }

        protected override IIndexModel GetIndexModel()
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
    }
}
