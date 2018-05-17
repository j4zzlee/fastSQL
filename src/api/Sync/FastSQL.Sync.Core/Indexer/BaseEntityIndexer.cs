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
    public abstract class BaseEntityIndexer : BaseIndexer, IEntityIndexer
    {
        protected readonly IProcessor Processor;
        protected readonly IRichProvider Provider;
        private readonly IRichAdapter Adapter;
        protected readonly EntityRepository EntityRepository;
        private readonly ConnectionRepository ConnectionRepository;
        protected EntityModel EntityModel;
        protected ConnectionModel ConnectionModel;

        public BaseEntityIndexer(
            IProcessor processor,
            IOptionManager optionManager,
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

        public override IIndexer SetIndex(IIndexModel model)
        {
            EntityModel = model as EntityModel;
            SpreadOptions();
            return this;
        }

        protected override IIndexModel GetIndexer()
        {
            return EntityModel;
        }
        
        protected override BaseRepository GetRepository()
        {
            return EntityRepository;
        }
        
        protected virtual IIndexer SpreadOptions()
        {
            ConnectionModel = ConnectionRepository.GetById(EntityModel.SourceConnectionId.ToString());
            var connectionOptions = ConnectionRepository.LoadOptions(ConnectionModel.Id.ToString());
            var connectionOptionItems = connectionOptions.Select(c => new OptionItem { Name = c.Key, Value = c.Value });
            Adapter.SetOptions(connectionOptionItems);
            Provider.SetOptions(connectionOptionItems);
            return this;
        }

    }
}
