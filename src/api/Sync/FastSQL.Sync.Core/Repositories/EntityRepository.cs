using FastSQL.Sync.Core.Models;
using st2forget.utils.sql;
using System.Data.Common;

namespace FastSQL.Sync.Core.Repositories
{
    public class EntityRepository : BaseGenericRepository<EntityModel>
    {
        public EntityRepository(DbConnection connection) : base(connection)
        {
        }
    }
}
