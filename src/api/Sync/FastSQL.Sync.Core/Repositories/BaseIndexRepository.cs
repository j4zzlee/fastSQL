using Dapper;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Filters;
using FastSQL.Sync.Core.Models;
using st2forget.commons.datetime;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace FastSQL.Sync.Core.Repositories
{
    public abstract class BaseIndexRepository<T> : BaseGenericRepository<T>
        where T : class, IIndexModel, new()
    {
        protected BaseIndexRepository(DbConnection connection) : base(connection)
        {
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
                    var paramName = StringRandom.Generate(10);
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

            return result.Select(d => IndexItemModel.FromDictionary(d));
        }

        public IndexItemModel GetIndexedItemById(IIndexModel model, string id)
        {
            var items = _connection.Query($@"
SELECT * FROM [{model.ValueTableName}]
WHERE [Id] = @Id", param: new { Id = id }, transaction: _transaction) as IEnumerable<IDictionary<string, object>>;
            var item = items?.FirstOrDefault();
            return item != null ? IndexItemModel.FromDictionary(item) : null;
        }

        public IndexItemModel GetIndexedItemBySourceId(IIndexModel model, string id)
        {
            var items = _connection.Query($@"
SELECT * FROM [{model.ValueTableName}]
WHERE [SourceId] = @Id", param: new { Id = id }, transaction: _transaction) as IEnumerable<IDictionary<string, object>>;
            var item = items?.FirstOrDefault();
            return item != null ? IndexItemModel.FromDictionary(item) : null;
        }

        public IndexItemModel GetIndexedItemDestinationId(IIndexModel model, string id)
        {
            var items = _connection.Query($@"
SELECT * FROM [{model.ValueTableName}]
WHERE [DestinationId] = @Id", param: new { Id = id }, transaction: _transaction) as IEnumerable<IDictionary<string, object>>;
            var item = items?.FirstOrDefault();
            return item != null ? IndexItemModel.FromDictionary(item) : null;
        }


    }
}
