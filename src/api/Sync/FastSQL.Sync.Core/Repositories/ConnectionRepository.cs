using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;

namespace FastSQL.Sync.Core.Repositories
{
    public class ConnectionRepository : BaseRepository
    {
        public ConnectionRepository(DbConnection connection, DbTransaction transaction) : base(connection, transaction)
        {
        }

        public void LinkOptions(Guid id, IEnumerable<OptionItem> options)
        {
            base.LinkOptions<ConnectionModel>(id, EntityType.Connection, options);
        }

        public IEnumerable<OptionModel> LoadOptions(IEnumerable<ConnectionModel> connections)
        {
            return LoadOptions(connections.Select(c => c.Id), EntityType.Connection);
        }
    }
}
