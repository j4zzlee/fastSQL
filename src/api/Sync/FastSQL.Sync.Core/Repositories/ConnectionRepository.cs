using System.Data.Common;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;

namespace FastSQL.Sync.Core.Repositories
{
    public class ConnectionRepository : BaseGenericRepository<ConnectionModel>
    {
        public ConnectionRepository(DbConnection connection) : base(connection)
        {
        }

        protected override EntityType EntityType => EntityType.Connection;
    }
}
