using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FastSQL.Sync.Core.Queuers
{
    public class QueueChangesManager
    {
        private IIndexModel _indexerModel;
        private Action<string> _reporter;
        private readonly EntityRepository entityRepository;
        private readonly AttributeRepository attributeRepository;

        public void SetIndex(IIndexModel model)
        {
            _indexerModel = model;
        }

        public QueueChangesManager(EntityRepository entityRepository,
            AttributeRepository attributeRepository)
        {
            this.entityRepository = entityRepository;
            this.attributeRepository = attributeRepository;
        }

        public void OnReport(Action<string> reporter)
        {
            _reporter = reporter;
        }

        private void Report(string message)
        {
            //_messages.Add(message);
            _reporter?.Invoke(message);
        }

        public async Task QueueChanges()
        {
            await Task.Run(() =>
            {
                var limit = 100;
                var offset = 0;
                var dependencies = entityRepository.GetDependencies(_indexerModel.Id, _indexerModel.EntityType);
                var dependsOnEntities = entityRepository.GetByIds(dependencies.Where(d => d.TargetEntityType == EntityType.Entity).Select(d => d.TargetEntityId.ToString()))
                    .Select(e => e as IIndexModel);
                var dependsOnAttributes = attributeRepository.GetByIds(dependencies.Where(d => d.TargetEntityType == EntityType.Attribute).Select(d => d.TargetEntityId.ToString()))
                    .Select(e => e as IIndexModel);
                var dependsOnIndexes = dependsOnEntities.Union(dependsOnAttributes);
                while (true)
                {
                    var changedItems = entityRepository.GetIndexChangedItems(_indexerModel, limit, offset, out int totalcount);
                    if (changedItems.Count() <= 0)
                    {
                        break;
                    }

                    // Items that has been changed OR not synced yet and should be valid
                    foreach (var item in changedItems)
                    {
                        var relatedItemNotSynced = false;
                        var relatedItemNotFound = false;
                        if (_indexerModel.EntityType == EntityType.Attribute)
                        {
                            var attributeModel = (AttributeModel)_indexerModel;
                            var entityModel = entityRepository.GetById(attributeModel.EntityId.ToString());
                            var entityIndexItem = entityRepository.GetIndexedItemBySourceId(entityModel, item.GetSourceId());
                            if (entityIndexItem == null || !entityIndexItem.HasValues)
                            {
                                relatedItemNotFound = true;
                            }
                            else if (string.IsNullOrWhiteSpace(entityIndexItem.GetDestinationId()))
                            {
                                relatedItemNotSynced = true;
                            }

                            if (relatedItemNotSynced)
                            {
                                /**
                                 * Adding these status only for reporting
                                 */
                                entityRepository.AddIndexItemState(
                                    _indexerModel.ValueTableName,
                                    item.GetId(), relatedItemNotFound ? ItemState.RelatedItemNotFound : ItemState.RelatedItemNotSynced);
                                continue;
                            }
                        }
                        // Items that has dependencies that are not resolved/synced should not be queued
                        var shouldNotSync = false;
                        foreach (var dependence in dependencies)
                        {
                            var model = dependsOnIndexes.FirstOrDefault(d => d.Id == dependence.TargetEntityId && d.EntityType == dependence.TargetEntityType);
                            var dependsOnItem = entityRepository.GetDependsOnItem(model.ValueTableName, dependence, item);
                            if (dependsOnItem == null || !dependsOnItem.HasValues)
                            {
                                // should let item wait for it
                                relatedItemNotFound = true;
                            }
                            else if (string.IsNullOrWhiteSpace(dependsOnItem["DestinationId"]?.ToString()))
                            {
                                // should let item wait for it
                                relatedItemNotSynced = true;
                            }
                            else if (dependsOnItem.HasState(ItemState.Failed | ItemState.ValidationFailed | ItemState.Invalid | ItemState.RelatedItemNotFound | ItemState.RelatedItemNotSynced))
                            {
                                shouldNotSync = true;
                            }

                            if (relatedItemNotSynced || relatedItemNotFound || shouldNotSync)
                            {
                                break;
                            }
                        }

                        if (relatedItemNotSynced)
                        {
                            /**
                             * Adding these status only for reporting
                             */
                            entityRepository.AddIndexItemState(
                                _indexerModel.ValueTableName,
                                item.GetId(), relatedItemNotFound ? ItemState.RelatedItemNotFound : ItemState.RelatedItemNotSynced);
                            continue;
                        }

                        if (shouldNotSync)
                        {
                            continue;
                        }

                        // TODO: what if retrycount > 1000?
                        // TODO: what about Failed and ValidationFailed
                        entityRepository.RemoveIndexItemState(
                                _indexerModel.ValueTableName,
                                item.GetId(), ItemState.RelatedItemNotFound | ItemState.RelatedItemNotSynced);

                        // only valid item can be queued
                        entityRepository.QueueItem(_indexerModel, item.GetId());
                    }
                    offset += limit;
                }
            });
        }
    }
}
