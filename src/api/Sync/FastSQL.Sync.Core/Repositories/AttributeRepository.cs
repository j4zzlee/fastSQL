using Dapper;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using st2forget.utils.sql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace FastSQL.Sync.Core.Repositories
{
    public class AttributeRepository : BaseGenericRepository<AttributeModel>
    {
        public AttributeRepository(DbConnection connection) : base(connection)
        {
        }

        public IEnumerable<DependencyItemModel> GetDependencies(Guid id)
        {
            return _connection.Query<DependencyItemModel>($@"SELECT * FROM [core_entity_dependency] 
WHERE [EntityId] = @EntityId AND [EntityType] = @EntityType",
new { EntityId = id, EntityType = EntityType.Attribute },
transaction: _transaction);
        }

        public IEnumerable<DependencyItemModel> GetDependencies(Guid id, EntityType targetEntityType)
        {
            return _connection.Query<DependencyItemModel>($@"SELECT * FROM [core_entity_dependency] 
WHERE [EntityId] = @EntityId AND [EntityType] = @EntityType AND [TargetEntityType] = @TargetEntityType",
new { EntityId = id, EntityType = EntityType.Attribute, TargetEntityType = targetEntityType },
transaction: _transaction);
        }
        
        public void RemoveDependencies(Guid id)
        {
            _connection.Execute($@"DELETE FROM [core_entity_dependency] 
WHERE [EntityId] = @EntityId AND [EntityType] = @EntityType",
new { EntityId = id, EntityType = EntityType.Attribute },
transaction: _transaction);
        }

        public void SetDependencies(Guid id, IEnumerable<DependencyItemModel> dependencies)
        {
            foreach (var dependency in dependencies)
            {
                dependency.EntityId = id;
                dependency.EntityType = EntityType.Attribute;
            }
            _connection.Execute($@"DELETE FROM [core_entity_dependency] 
WHERE [EntityId] = @EntityId AND [EntityType] = @EntityType",
new { EntityId = id, EntityType = EntityType.Attribute },
transaction: _transaction);
            _connection.Execute($@"INSERT INTO [core_entity_dependency](
[{nameof(DependencyItemModel.EntityId)}],
[{nameof(DependencyItemModel.EntityType)}],
[{nameof(DependencyItemModel.TargetEntityId)}],
[{nameof(DependencyItemModel.TargetEntityType)}],
[{nameof(DependencyItemModel.StepToExecute)}],
[{nameof(DependencyItemModel.DependOnStep)}],
[{nameof(DependencyItemModel.ExecuteImmediately)}])
VALUES (
@{nameof(DependencyItemModel.EntityId)},
@{nameof(DependencyItemModel.EntityType)},
@{nameof(DependencyItemModel.TargetEntityId)},
@{nameof(DependencyItemModel.TargetEntityType)},
@{nameof(DependencyItemModel.StepToExecute)},
@{nameof(DependencyItemModel.DependOnStep)},
@{nameof(DependencyItemModel.ExecuteImmediately)})",
dependencies,
transaction: _transaction);
        }
    }
}
