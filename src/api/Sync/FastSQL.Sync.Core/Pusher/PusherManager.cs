using DateTimeExtensions;
using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Indexer;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Puller;
using FastSQL.Sync.Core.Repositories;
using Newtonsoft.Json;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.Sync.Core.Pusher
{
    public class PusherManager: IDisposable
    {
        private List<string> _messages;
        private Action<string> _reporter;
        private IIndexModel _indexerModel;
        private IIndexer _indexer;
        private IPusher _pusher;
        private IEventAggregator eventAggregator;
        private readonly IEnumerable<IEntityPuller> entityPullers;
        private readonly IEnumerable<IAttributePuller> attributePullers;
        public ResolverFactory ResolverFactory { get; set; }
        public RepositoryFactory RepositoryFactory { get; set; }

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

        public PusherManager(
            IEventAggregator eventAggregator,
            IEnumerable<IEntityPuller> entityPullers,
            IEnumerable<IAttributePuller> attributePullers)
        {
            this.eventAggregator = eventAggregator;
            this.entityPullers = entityPullers;
            this.attributePullers = attributePullers;
            _messages = new List<string>();
        }

        public async Task<PushState> PushItem(IndexItemModel item)
        {
            Report($@"
---------------------------------------------------------------------------------
Begin synchronizing item {JsonConvert.SerializeObject(item, Formatting.Indented)}...");
            _pusher.SetIndex(_indexerModel);
            _pusher.SetItem(item);
            var result = await Task.Run(() =>
            {
                var entityRepository = RepositoryFactory.Create<EntityRepository>(this);
                var attributeRepository = RepositoryFactory.Create<AttributeRepository>(this);
                var messageRepository = RepositoryFactory.Create<MessageRepository>(this);

                try
                {
                    entityRepository.BeginTransaction();
                    attributeRepository.BeginTransaction();
                    messageRepository.BeginTransaction();
                    var pushState = PushState.Success;
                    var destinationId = item.GetDestinationId();
                    if (!string.IsNullOrWhiteSpace(destinationId))
                    {
                        if (item.HasState(ItemState.Removed))
                        {
                            pushState = _pusher.Remove();
                        }
                        else
                        {
                            pushState = _pusher.Update();
                        }
                    }
                    else
                    {
                        destinationId = _pusher.GetDestinationId();
                        if (!string.IsNullOrWhiteSpace(destinationId))
                        {
                            if (item.HasState(ItemState.Removed))
                            {
                                pushState = _pusher.Remove(destinationId);
                            }
                            else
                            {
                                pushState = _pusher.Update(destinationId);
                            }
                        }
                        else // still cannot find a destinationId, which means the entity/attribute does not exists
                        {
                            pushState = _pusher.Create(out destinationId);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(destinationId) && (pushState & PushState.Success) > 0)
                    {
                        entityRepository.UpdateItemDestinationId(_indexerModel, item.GetSourceId(), destinationId);
                        // Detect dependencies that depends on "Synced" step
                        UpdateDependencies(item, destinationId);
                    }
                    else
                    {
                        // Signal to tell that the item is success or not
                        entityRepository.Retry(_indexerModel, item.GetId(), pushState);
                    }

                    entityRepository.Commit();
                    attributeRepository.Commit();
                    return pushState;
                }
                catch
                {
                    entityRepository.RollBack();
                    attributeRepository.RollBack();
                    // Update invalid item
                    // Increate retry count
                    // Next time when queue items, it will be put behind because of the retry count
                    entityRepository.Retry(_indexerModel, item.GetId(), PushState.UnexpectedError);
                    throw;
                }
                finally
                {
                    entityRepository?.Dispose();
                    attributeRepository?.Dispose();
                    messageRepository?.Dispose();
                    Report($@"
Ended synchronizing...
---------------------------------------------------------------------------------
");
                }
            });
            return result;
        }

        public async Task Push(params IndexItemModel[] items)
        {
            if (items == null)
            {
                return;
            }
            foreach (var item in items)
            {
                await PushItem(item);
            }
        }

        private void UpdateDependencies(IndexItemModel item, string destinationId)
        {
            using (var entityRepository = RepositoryFactory.Create<EntityRepository>(this))
            using (var attributeRepository = RepositoryFactory.Create<AttributeRepository>(this))
            {
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

                        attributeRepository.UpdateItemDestinationId(attr, item.GetSourceId(), destinationId);
                        Report($"Done updating destination ID for attribute {attr.Name}.");
                    }
                }

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

        public virtual void Dispose()
        {
            RepositoryFactory.Release(this);
        }
    }
}
