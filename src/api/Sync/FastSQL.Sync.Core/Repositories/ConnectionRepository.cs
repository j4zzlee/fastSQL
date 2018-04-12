using System.Data.Common;
using FastSQL.Sync.Core.Models;

namespace FastSQL.Sync.Core.Repositories
{
    public class ConnectionRepository : BaseGenericRepository<ConnectionModel>
    {
        public ConnectionRepository(DbConnection connection, DbTransaction transaction) : base(connection, transaction)
        {
        }
    }
}
