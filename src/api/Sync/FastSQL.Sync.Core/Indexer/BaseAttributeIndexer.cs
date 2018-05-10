using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Indexer
{
    public class BaseAttributeIndexer : BaseIndexer, IAttributeIndexer
    {
        protected readonly IProcessor AttributeProcessor;
        protected readonly IProcessor EntityProcessor;
        protected readonly IRichProvider Provider;
        protected readonly EntityRepository EntityRepository;
        protected readonly AttributeRepository AttributeRepository;
        protected EntityModel EntityModel;
        protected AttributeModel AttributeModel;

        public BaseAttributeIndexer(
            IProcessor attributeProcessor,
            IProcessor entityProcessor,
            IRichProvider provider,
            IOptionManager optionManager,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository) : base(optionManager)
        {
            this.AttributeProcessor = attributeProcessor;
            this.EntityProcessor = entityProcessor;
            this.Provider = provider;
            EntityRepository = entityRepository;
            AttributeRepository = attributeRepository;
        }

        public virtual IAttributeIndexer SetAttribute(AttributeModel attribute, EntityModel entity)
        {
            AttributeModel = attribute;
            EntityModel = entity;
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
    }
}
