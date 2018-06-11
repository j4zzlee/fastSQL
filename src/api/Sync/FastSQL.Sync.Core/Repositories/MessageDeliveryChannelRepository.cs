using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace FastSQL.Sync.Core.Repositories
{
    public class MessageDeliveryChannelRepository : BaseGenericRepository<MessageDeliveryChannelModel>
    {
        public MessageDeliveryChannelRepository(DbConnection connection) : base(connection)
        {
        }

        protected override EntityType EntityType => EntityType.MessageDeliveryChannelModel;
    }
}
