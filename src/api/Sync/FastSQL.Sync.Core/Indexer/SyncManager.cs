using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using Newtonsoft.Json;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.Sync.Core.Indexer
{
    public class SyncManager
    {
        private List<string> _messages;
        private Action<string> _reporter;
        private IIndexModel _indexerModel;
        private IIndexer _indexer;
        private IPusher _pusher;
        private IEventAggregator eventAggregator;
        private readonly IEnumerable<IEntityPuller> entityPullers;
        private readonly IEnumerable<IAttributePuller> attributePullers;
        private ResolverFactory resolverFactory;
        private readonly EntityRepository entityRepository;
        private readonly AttributeRepository attributeRepository;
        private readonly ConnectionRepository connectionRepository;

        public void SetIndex(IIndexModel model)
        {
            _indexerModel = model;
        }

        public void SetIndexer(IIndexer indexer)
        {
            _indexer = indexer;
            _indexer.OnReport(Report);
        }

        public void SetPusher(IPusher pusher)
        {
            _pusher = pusher;
            _pusher.OnReport(Report);
        }

        public void OnReport(Action<string> reporter)
        {
            _reporter = reporter;
        }

        private void Report(string message)
        {
            _messages.Add(message);
            _reporter?.Invoke(message);
        }

        public IEnumerable<string> GetReportMessages()
        {
            return _messages ?? new List<string>();
        }

        public SyncManager(
            IEventAggregator eventAggregator,
            IEnumerable<IEntityPuller> entityPullers,
            IEnumerable<IAttributePuller> attributePullers,
            ResolverFactory resolverFactory,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            ConnectionRepository connectionRepository)
        {
            this.eventAggregator = eventAggregator;
            this.entityPullers = entityPullers;
            this.attributePullers = attributePullers;
            this.resolverFactory = resolverFactory;
            this.entityRepository = entityRepository;
            this.attributeRepository = attributeRepository;
            this.connectionRepository = connectionRepository;
            _messages = new List<string>();
        }

        //private void InitPusher()
        //{
        //    if (_indexerModel.EntityType == EntityType.Entity)
        //    {
        //        var entity = _indexerModel as EntityModel;
        //        (_pusher as IEntityPuller).SetEntity(entity);
        //    } else
        //    {
        //        var attr = _indexerModel as AttributeModel;
        //        var entity = entityRepository.GetById(attr.EntityId.ToString());
        //        (_pusher as IAttributePusher).SetAttribute(attr, entity);
        //    }
        //}

        public async Task PushSingle(IndexItemModel item)
        {
            Report($@"
---------------------------------------------------------------------------------
Begin synchronizing item {JsonConvert.SerializeObject(item, Formatting.Indented)}...");
            await Task.Run(() =>
            {
                try
                {
                    //InitPusher();

                    entityRepository.BeginTransaction();
                    attributeRepository.BeginTransaction();

                    _pusher
                    .SetItem(item);
                    var destinationId = item.GetDestinationId();
                    if (!string.IsNullOrWhiteSpace(destinationId))
                    {
                        if (item.HasState(ItemState.Removed))
                        {
                            _pusher.Remove();
                        }
                        else
                        {
                            _pusher.Update();
                        }
                    }
                    else
                    {
                        destinationId = _pusher.GetDestinationId();
                        if (!string.IsNullOrWhiteSpace(destinationId))
                        {
                            if (item.HasState(ItemState.Removed))
                            {
                                _pusher.Remove(destinationId);
                            }
                            else
                            {
                                _pusher.Update(destinationId);
                            }
                        }
                        else // still cannot find a destinationId, which means the entity/attribute does not exists
                        {
                            destinationId = _pusher.Create();
                        }
                    }

                    if (string.IsNullOrWhiteSpace(destinationId))
                    {
                        throw new NotSupportedException($@"There is something wrong when pushing data of
{JsonConvert.SerializeObject(item, Formatting.Indented)}
to destination. Please make sure that the Pusher of {_indexerModel.Name} works correctly and got Destination ID after synced.");
                    }

                    // Update Changed from Yes to No
                    // Update DestinationId
                    UpdateDestinationId(item, destinationId);

                    // Detect dependencies that depends on "Synced" step
                    UpdateDependencies(item, destinationId);

                    entityRepository.Commit();
                    attributeRepository.Commit();
                }
                catch
                {
                    entityRepository.RollBack();
                    attributeRepository.RollBack();
                    // Update invalid item
                    // Increate retry count
                    // Next time when queue items, it will be put behind because of the retry count
                    entityRepository.UpdateFailedIndexItem(_indexerModel, item.GetId());
                    throw;
                }
                finally
                {
                    Report($@"
Ended synchronizing...
---------------------------------------------------------------------------------
");
                }
            });
            
        }

        public async Task Push(params IndexItemModel[] items)
        {
            if (items == null)
            {
                return;
            }
            foreach (var item in items)
            {
                await PushSingle(item);
            }
        }

        private void UpdateDestinationId(IndexItemModel item, string destinationId)
        {
            entityRepository.UpdateItemDestinationId(_indexerModel, item.GetSourceId(), destinationId, true);

            if (_indexerModel.EntityType == EntityType.Entity)
            {
                // Update Attribute Mapping (DestinationId) if _indexerModel is Entity
                var attrs = attributeRepository.GetByEntityId(_indexerModel.Id.ToString());
                foreach (var attr in attrs)
                {
                    Report($"Updating destination ID for attribute {attr.Name}.");
                    if (!attributeRepository.Initialized(attr))
                    {
                        Report($"Attribute {attr.Name} is not initialized.");
                        continue;
                    }

                    if (attr.HasState(EntityState.Disabled))
                    {
                        Report($"Attribute {attr.Name} is Disabled.");
                        continue;
                    }

                    attributeRepository.UpdateItemDestinationId(attr, item.GetSourceId(), destinationId, false);
                    Report($"Done updating destination ID for attribute {attr.Name}.");
                }
            }

        }

        private void UpdateDependencies(IndexItemModel item, string destinationId)
        {
            var dependencies = entityRepository.GetDependenciesOn(_indexerModel.Id, _indexerModel.EntityType);
            foreach (var dependency in dependencies)
            {
                if (dependency.HasDependOnStep(IntegrationStep.Pushing | IntegrationStep.Pushing))
                {
                    if (dependency.HasExecutionStep(IntegrationStep.Indexing | IntegrationStep.Indexed))
                    {
                        // Only add the signal to tell that the dependant entity should be pull (via PullNext) based on this item
                        entityRepository.AddPullDependency(
                            dependency.EntityId,
                            dependency.EntityType,
                            _indexerModel.Id,
                            _indexerModel.EntityType,
                            item.GetId());
                    }
                }
            }
        }
    }
}
