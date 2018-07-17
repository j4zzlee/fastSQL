using Dapper;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Filters;
using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace FastSQL.Sync.Core.Repositories
{
    public class QueueItemRepository : BaseGenericRepository<QueueItemModel>
    {
        public QueueItemRepository(DbConnection connection) : base(connection)
        {
        }

        protected override EntityType EntityType => EntityType.QueueItem;

        public IEnumerable<QueueItemModel> FilterQueueItems(IEnumerable<FilterArgument> filters,
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
                    if (filter.FilterType == FilterType.Expression)
                    {
                        var paramName = StringExtensions.StringExtensions.Random(10);
                        filterStrs.Add($@"({filter.Field}) {filter.Op} @Param_{paramName}");
                        @params.Add($@"Param_{paramName}", filter.Target);
                    } else
                    {
                        var paramName = StringExtensions.StringExtensions.Random(10);
                        filterStrs.Add($@"[{filter.Field}] {filter.Op} @Param_{paramName}");
                        @params.Add($@"Param_{paramName}", filter.Target);
                    }
                    
                }
                //var condition = string.Join(" AND ", filters.Select(f => $@"[{f.Field}] {f.Op} {}"));
                @where = string.Join(" AND ", filterStrs);
                if (!string.IsNullOrWhiteSpace(@where))
                {
                    @where = $"WHERE {@where}";
                }
            }
            var sql = $@"
SELECT * FROM [core_queue_items]
{@where}
ORDER BY [CreatedAt], [Id]
OFFSET @Offset ROWS
FETCH NEXT @Limit ROWS ONLY;
";
            var countSql = $@"
SELECT COUNT(*) FROM [core_queue_items]
{@where}
";
            totalCount = _connection
                .Query<int>(countSql, param: @params, transaction: _transaction)
                .FirstOrDefault();
            var result = _connection
                .Query<QueueItemModel>(sql, param: @params, transaction: _transaction);
            return result;
        }

        public IEnumerable<QueueItemModel> GetQueuedItemsByStatus(
            PushState state, 
            PushState exclude, 
            int limit, 
            int offset,
            bool includeEmptyStatus = false,
            bool excludeEmptyStatus = true)
        {
            return _connection.Query<QueueItemModel>($@"
SELECT * 
FROM [core_queue_items]
WHERE (
    (([Status] IS NULL OR [Status] = 0) AND (@IncludeEmptyStatus = 1 OR @Status = 0)) -- Get empty status items
    OR ([Status] & @Status) > 0 -- Get status items that matched
)
AND ( -- exclude
    ([Status] IS NOT NULL AND [Status] <> 0 AND (@ExcludeEmptyStatus = 1 OR @Exclude = 0))
    OR ([Status] & @Exclude) = 0 -- Exclude items that matched
)
ORDER BY [CreatedAt] DESC
OFFSET @Offset ROWS
FETCH NEXT @Limit ROWS ONLY;
",
param: new
{
    Limit = limit, 
    Offset = offset,
    Status = state,
    Exclude = exclude,
    IncludeEmptyStatus = includeEmptyStatus ? 1 : 0,
    ExcludeEmptyStatus = excludeEmptyStatus ? 1 : 0
},
transaction: _transaction);
        }
    }
}
