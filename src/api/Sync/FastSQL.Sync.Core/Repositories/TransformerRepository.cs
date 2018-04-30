using System;
using System.Collections.Generic;
using System.Data.Common;
using Dapper;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using st2forget.utils.sql;

namespace FastSQL.Sync.Core.Repositories
{
    public class TransformerRepository : BaseGenericRepository<ColumnTransformationModel>
    {
        public TransformerRepository(DbConnection connection) : base(connection)
        {
        }

        public IEnumerable<ColumnTransformationModel> GetTransformations(Guid entityId, EntityType entityType)
        {
            return _connection.Query<ColumnTransformationModel>($@"SELECT * FROM [core_entity_column_transformation]
WHERE [TargetEntityId] = @entityId AND [TargetEntityType] = @entityType", new { entityId, entityType});
        }
    }
}
