using Dapper;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Filters;
using FastSQL.Sync.Core.Models;
using DateTimeExtensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace FastSQL.Sync.Core.Repositories
{
    public abstract class BaseIndexRepository<T> : BaseGenericRepository<T>
        where T : class, IIndexModel, new()
    {
        protected BaseIndexRepository(DbConnection connection) : base(connection)
        {
        }

        public void AddIndexItemState(string valueTable, string itemId, ItemState state)
        {
            _connection.Execute($@"
UPDATE [{valueTable}]
SET [State] = CASE  
    WHEN [State] = 0 THEN @State
    WHEN [State] IS NULL THEN @State 
    ELSE ([State] | @State)
    END
WHERE [Id] = @ItemId
",
new
{
    ItemId = itemId,
    State = state
},
transaction: _transaction);
        }

        public void RemoveIndexItemState(string valueTable, string itemId, ItemState state)
        {
            _connection.Execute($@"
UPDATE [{valueTable}]
SET [State] = CASE  
    WHEN [State] = 0 THEN 0
    WHEN [State] IS NULL THEN NULL
    ELSE ([State] | @State) ^ @State
    END
WHERE [Id] = @ItemId
",
new
{
    ItemId = itemId,
    State = state
},
transaction: _transaction);
        }

        public void ChangeIndexedItems(IIndexModel model, ItemState include, ItemState exclude, params string[] ids)
        {
            var sql = $@"
UPDATE {model.ValueTableName}
SET [State] = 
    CASE  
        WHEN [State] = 0 THEN @State
        WHEN [State] IS NULL THEN @State 
        ELSE ([State] | @State | @StatesToExclude) ^ @StatesToExclude
    END";
            if (ids != null && ids.Count() > 0)
            {
                sql += $@"
WHERE Id IN @Id";
            }
            _connection.Execute(sql, new
            {
                Id = ids,
                State = include | ItemState.Changed,
                StatesToExclude = exclude,
            }, transaction: _transaction);
        }

        public void ChangeIndexedItemsRange(IIndexModel model, ItemState include, ItemState exclude, string fromId = null, string toId = null)
        {
            fromId = !string.IsNullOrWhiteSpace(fromId) ? fromId : "0";

            var sql = $@"
UPDATE {model.ValueTableName}
SET [State] = 
    CASE  
        WHEN [State] = 0 THEN @State
        WHEN [State] IS NULL THEN @State 
        ELSE ([State] | @State | @StatesToExclude) ^ @StatesToExclude
    END
WHERE Id >= @FromId";
            if (!string.IsNullOrWhiteSpace(toId))
            {
                sql = $" AND Id < @ToId";
            }
            _connection.Execute(sql, new
            {
                FromId = fromId,
                ToId = toId,
                State = include | ItemState.Changed,
                StatesToExclude = exclude,
            }, transaction: _transaction);
        }

        public IEnumerable<IndexItemModel> GetIndexedItems(
            IIndexModel model,
            IEnumerable<FilterArgument> filters,
            int limit,
            int offset,
            out int totalCount)
        {
            var @params = new DynamicParameters();
            @params.Add("Limit", limit > 0 ? limit : 100);
            @params.Add("Offset", offset);
            var filterStrs = new List<string>();
            var @where = string.Empty;
            if (filters != null && filters.Count() > 0)
            {
                foreach (var filter in filters)
                {
                    var paramName = StringExtensions.StringExtensions.Random(10);
                    filterStrs.Add($@"[{filter.Field}] {filter.Op} @Param_{paramName}");
                    @params.Add($@"Param_{paramName}", filter.Target);
                }
                //var condition = string.Join(" AND ", filters.Select(f => $@"[{f.Field}] {f.Op} {}"));
                @where = string.Join(" AND ", filterStrs);
                if (!string.IsNullOrWhiteSpace(@where))
                {
                    @where = $"WHERE {@where}";
                }
            }
            var sql = $@"
SELECT * FROM {model.ValueTableName}
{@where}
ORDER BY [Id]
OFFSET @Offset ROWS
FETCH NEXT @Limit ROWS ONLY;
";
            var countSql = $@"
SELECT COUNT(*) FROM {model.ValueTableName}
{@where}
";
            totalCount = _connection
                .Query<int>(countSql, param: @params, transaction: _transaction)
                .FirstOrDefault();
            var result = _connection
                .Query(sql, param: @params, transaction: _transaction) as IEnumerable<IDictionary<string, object>>;

            return result.Select(d => IndexItemModel.FromJObject(JObject.FromObject(d)));
        }

        public IndexItemModel GetDependsOnItem(string valueTableName, DependencyItemModel dependence, IndexItemModel item)
        {
            var referenceKeyParams = Regex.Split(dependence.ReferenceKeys, "[,;|]", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var foreignKeyParams = Regex.Split(dependence.ForeignKeys, "[,;|]", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var @params = foreignKeyParams
                .Select((fk, i) => new { fk, i })
                .Select(fk => new KeyValuePair<string, string>(referenceKeyParams[fk.i], item[fk.fk].ToString()))
                .ToDictionary(fk => fk.Key, fk => fk.Value);
            var dynamicParams = new DynamicParameters();
            foreach (var p in @params)
            {
                dynamicParams.Add(p.Key, p.Value);
            }
            var sql = $@"
SELECT TOP 1 * FROM {valueTableName}
WHERE {string.Join(" AND ", @params.Select(p => $@"[{p.Key}] = @{p.Key}"))}
";
            var dependsOnItem = _connection.Query<object>(sql, param: dynamicParams, transaction: _transaction)
                .Select(d => IndexItemModel.FromJObject(JObject.FromObject(d))).FirstOrDefault();
            return dependsOnItem;
        }

        public IEnumerable<IndexItemModel> GetIndexChangedItems(
            IIndexModel model,
            int limit,
            int offset,
            out int totalCount)
        {
            var @params = new DynamicParameters();
            @params.Add("Limit", limit > 0 ? limit : 100);
            @params.Add("Offset", offset);
            @params.Add("ChangedState", ItemState.Changed);
            /**
             * Do not ignore RelatedItemNotFound and RelatedItemNotSync, 
             * These items should be checked in code logic
             */
            @params.Add("ExcludeStates", ItemState.Invalid | ItemState.Processed);
            var sql = $@"
SELECT v.* 
FROM {model.ValueTableName} v
WHERE 
    v.[DestinationId] IS NULL
    OR v.[State] IS NULL
    OR (
        (v.[State] & @ChangedState) > 0
        AND (
            (v.[State] & @ExcludeStates) = 0
        )
    )
ORDER BY v.[Id], v.[RetryCount]
OFFSET @Offset ROWS
FETCH NEXT @Limit ROWS ONLY;
";
            var countSql = $@"
SELECT COUNT(v.Id) 
FROM {model.ValueTableName} v
WHERE 
    v.[DestinationId] IS NULL
    OR v.[State] IS NULL
    OR (
        (v.[State] & @ChangedState) > 0
        AND (
            (v.[State] & @ExcludeStates) = 0
        )
    )
";
            totalCount = _connection
                .Query<int>(countSql, param: @params, transaction: _transaction)
                .FirstOrDefault();
            var result = _connection
                .Query(sql, param: @params, transaction: _transaction) as IEnumerable<IDictionary<string, object>>;

            return result.Select(d => IndexItemModel.FromJObject(JObject.FromObject(d)));
        }

        public IndexItemModel GetIndexedItemById(IIndexModel model, string id)
        {
            var items = _connection.Query($@"
SELECT * FROM [{model.ValueTableName}]
WHERE [Id] = @Id", param: new { Id = id }, transaction: _transaction) as IEnumerable<IDictionary<string, object>>;
            var item = items?.FirstOrDefault();
            return item != null ? IndexItemModel.FromJObject(JObject.FromObject(item)) : null;
        }

        public IndexItemModel GetIndexedItemBySourceId(IIndexModel model, string id)
        {
            var items = _connection.Query($@"
SELECT * FROM [{model.ValueTableName}]
WHERE [SourceId] = @Id", param: new { Id = id }, transaction: _transaction) as IEnumerable<IDictionary<string, object>>;
            var item = items?.FirstOrDefault();
            return item != null ? IndexItemModel.FromJObject(JObject.FromObject(item)) : null;
        }

        public IndexItemModel GetIndexedItemDestinationId(IIndexModel model, string id)
        {
            var items = _connection.Query($@"
SELECT * FROM [{model.ValueTableName}]
WHERE [DestinationId] = @Id", param: new { Id = id }, transaction: _transaction) as IEnumerable<IDictionary<string, object>>;
            var item = items?.FirstOrDefault();
            return item != null ? IndexItemModel.FromJObject(JObject.FromObject(item)) : null;
        }

        public string QueueItem(IIndexModel model, string itemId, bool force = false)
        {
            if (force) // use for requeue errors
            {
                return Create<QueueItemModel>(new
                {
                    TargetEntityId = model.Id,
                    TargetEntityType = model.EntityType,
                    TargetItemId = itemId,
                    CreatedAt = DateTime.Now.ToUnixTimestamp(),
                    UpdatedAt = DateTime.Now.ToUnixTimestamp(),
                    RetryCount = 0,
                    Status = PushState.None
                });
            }
            // Get the newest
            var exists = _connection.Query<QueueItemModel>($@"
SELECT TOP 1 * 
FROM [core_queue_items]
WHERE [TargetEntityId] = @TargetEntityId 
    AND [TargetEntityType] = @TargetEntityType 
    AND [TargetItemId] = @TargetItemId
ORDER BY [CreatedAt] DESC -- always get the newest
",
param: new
{
    TargetEntityId = model.Id,
    TargetEntityType = model.EntityType,
    TargetItemId = itemId
},
transaction: _transaction).FirstOrDefault();

            if (exists != null && (exists.Status == 0 || exists.HasState(PushState.ByPassed | PushState.Failed | PushState.UnexpectedError)))
            {
                return null; // no row inserted
            }

            return Create<QueueItemModel>(new
            {
                TargetEntityId = model.Id,
                TargetEntityType = model.EntityType,
                TargetItemId = itemId,
                CreatedAt = DateTime.Now.ToUnixTimestamp(),
                UpdatedAt = DateTime.Now.ToUnixTimestamp(),
                RetryCount = 0,
                Status = PushState.None
            });
        }

        public QueueItemModel GetCurrentQueuedItems()
        {
            return _connection.Query<QueueItemModel>($@"
SELECT TOP 1 * 
FROM [core_queue_items]
WHERE (
    [Status] IS NULL 
    OR [Status] = 0
)
ORDER BY [CreatedAt] ASC -- always first in first out
",
 transaction: _transaction).FirstOrDefault();
        }
    }
}
