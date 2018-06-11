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

        public IEnumerable<QueueItemModel> GetQueuedItemByStatus(QueueItemState success)
        {
            throw new NotImplementedException();
        }
    }
}
