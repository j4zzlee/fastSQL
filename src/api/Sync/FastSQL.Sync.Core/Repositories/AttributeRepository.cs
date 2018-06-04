using Dapper;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using System.Collections.Generic;
using System.Data.Common;

namespace FastSQL.Sync.Core.Repositories
{
    public class AttributeRepository : BaseIndexRepository<AttributeModel>
    {
        public AttributeRepository(DbConnection connection) : base(connection)
        {
        }

        public IEnumerable<AttributeModel> GetByEntityId(string entityId)
        {
            var sql = $@"
SELECT * FROM [core_attributes]
WHERE [EntityId] = @EntityId
";
            return _connection.Query<AttributeModel>(sql, param: new
            {
                EntityId = entityId
            }, transaction: _transaction);
        }

        protected override EntityType EntityType => EntityType.Attribute;
    }
}
