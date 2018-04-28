using System.Data.Common;
using FastSQL.Sync.Core.Models;
using st2forget.utils.sql;

namespace FastSQL.Sync.Core.Repositories
{
    public class ConnectionRepository : BaseGenericRepository<ConnectionModel>
    {
        public ConnectionRepository(DbConnection connection) : base(connection)
        {
        }
    }
}
