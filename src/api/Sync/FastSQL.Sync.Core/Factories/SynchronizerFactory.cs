using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Indexer;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Puller;
using FastSQL.Sync.Core.Pusher;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastSQL.Sync.Core.Factories
{
    public class SynchronizerFactory: IDisposable
    {
        private readonly IEnumerable<IPuller> pullers;
        private readonly IEnumerable<IIndexer> indexers;
        private readonly IEnumerable<IPusher> pushers;
        public ResolverFactory ResolverFactory { get; set; }

        public SynchronizerFactory(
            IEnumerable<IPuller> pullers,
            IEnumerable<IIndexer> indexers,
            IEnumerable<IPusher> pushers)
        {
            this.pullers = pullers;
            this.indexers = indexers;
            this.pushers = pushers;
        }

        public IPuller CreatePuller(IIndexModel model)
        {
            if (model == null)
            {
                return null;
            }
            using (var connectionRepository = ResolverFactory.Resolve<ConnectionRepository>())
            using (var entityRepository = ResolverFactory.Resolve<EntityRepository>())
            {
                var sourceConnection = connectionRepository.GetById(model.SourceConnectionId.ToString());
                IPuller result = null;
                if (model.EntityType == EntityType.Attribute)
                {
                    var attrModel = model as AttributeModel;
                    var entityModel = entityRepository.GetById(attrModel.EntityId.ToString());
                    result = pullers.Where(p => typeof(IAttributePuller).IsAssignableFrom(p.GetType()))
                        .Select(p => p as IAttributePuller)
                        .FirstOrDefault(p => p.IsImplemented(model.SourceProcessorId, entityModel.SourceProcessorId, sourceConnection.ProviderId));
                }
                else
                {
                    result = pullers.Where(p => typeof(IEntityPuller).IsAssignableFrom(p.GetType()))
                        .Select(p => p as IEntityPuller)
                        .FirstOrDefault(p => p.IsImplemented(model.SourceProcessorId, sourceConnection.ProviderId));
                }
                var options = entityRepository.LoadOptions(model.Id.ToString(), model.EntityType)
                    .Select(o => new OptionItem
                    {
                        Name = o.Key,
                        Value = o.Value
                    });
                result.SetIndex(model);
                result.SetOptions(options);
                return result;
            }
        }

        public IPusher CreatePusher(IIndexModel model)
        {
            if (model == null)
            {
                return null;
            }
            using (var connectionRepository = ResolverFactory.Resolve<ConnectionRepository>())
            using (var entityRepository = ResolverFactory.Resolve<EntityRepository>())
            {
                var destinationConnection = connectionRepository.GetById(model.DestinationConnectionId.ToString());
                IPusher result = null;
                if (model.EntityType == EntityType.Attribute)
                {
                    var attrModel = model as AttributeModel;
                    var entityModel = entityRepository.GetById(attrModel.EntityId.ToString());
                    result = pushers
                        .Where(p => typeof(IAttributePusher).IsAssignableFrom(p.GetType()))
                        .Select(p => p as IAttributePusher)
                        .FirstOrDefault(p => p.IsImplemented(model.DestinationProcessorId, entityModel.DestinationProcessorId, destinationConnection.ProviderId));
                }
                else
                {
                    result = pushers
                        .Where(p => typeof(IEntityPusher).IsAssignableFrom(p.GetType()))
                        .Select(p => p as IEntityPusher)
                        .FirstOrDefault(p => p.IsImplemented(model.DestinationProcessorId, destinationConnection.ProviderId));
                }
                var options = entityRepository.LoadOptions(model.Id.ToString(), model.EntityType)
                    .Select(o => new OptionItem
                    {
                        Name = o.Key,
                        Value = o.Value
                    });
                result.SetIndex(model);
                result.SetOptions(options);
                return result;
            }
        }

        public IIndexer CreateIndexer(IIndexModel model)
        {
            if (model == null)
            {
                return null;
            }
            using (var connectionRepository = ResolverFactory.Resolve<ConnectionRepository>())
            using (var entityRepository = ResolverFactory.Resolve<EntityRepository>())
            {
                var sourceConnection = connectionRepository.GetById(model.SourceConnectionId.ToString());
                IIndexer result = null;
                if (model.EntityType == EntityType.Attribute)
                {
                    var attrModel = model as AttributeModel;
                    var entityModel = entityRepository.GetById(attrModel.EntityId.ToString());
                    result = indexers.Where(p => typeof(IAttributeIndexer).IsAssignableFrom(p.GetType()))
                        .Select(p => p as IAttributeIndexer)
                        .FirstOrDefault(p => p.IsImplemented(model.SourceProcessorId, entityModel.SourceProcessorId, sourceConnection.ProviderId));
                }
                else
                {
                    result = indexers.Where(p => typeof(IEntityIndexer).IsAssignableFrom(p.GetType()))
                        .Select(p => p as IEntityIndexer)
                        .FirstOrDefault(p => p.IsImplemented(model.SourceProcessorId, sourceConnection.ProviderId));
                }
                var options = entityRepository.LoadOptions(model.Id.ToString(), model.EntityType)
                    .Select(o => new OptionItem
                    {
                        Name = o.Key,
                        Value = o.Value
                    });
                result.SetIndex(model);
                result.SetOptions(options);
                return result;
            }
        }

        public void Dispose()
        {
            foreach(var i in indexers)
            {
                i?.Dispose();
            }

            foreach(var pu in pullers)
            {
                pu?.Dispose();
            }

            foreach (var ps in pushers)
            {
                ps?.Dispose();
            }
        }
    }
}
