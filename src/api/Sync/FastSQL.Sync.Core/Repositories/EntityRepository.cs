using FastSQL.Sync.Core.Models;
using System.Data.Common;

namespace FastSQL.Sync.Core.Repositories
{
    public class EntityRepository : BaseGenericRepository<EntityModel>
    {
        public EntityRepository(DbConnection connection, DbTransaction transaction) : base(connection, transaction)
        {
        }
    }
}
