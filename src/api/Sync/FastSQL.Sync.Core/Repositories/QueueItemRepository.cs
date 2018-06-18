using Dapper;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace FastSQL.Sync.Core.Repositories
{
    public class QueueItemRepository : BaseGenericRepository<QueueItemModel>
    {
        public QueueItemRepository(DbConnection connection) : base(connection)
        {
        }

        protected override EntityType EntityType => EntityType.QueueItem;

        public IEnumerable<QueueItemModel> GetQueuedItemsByStatus(
            QueueItemState state, 
            QueueItemState exclude, 
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

        public QueueItemModel GetItemToPush()
        {
            throw new NotImplementedException();
        }
    }
}
