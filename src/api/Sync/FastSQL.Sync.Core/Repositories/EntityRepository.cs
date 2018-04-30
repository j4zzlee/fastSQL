using Dapper;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using st2forget.utils.sql;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace FastSQL.Sync.Core.Repositories
{
    public class EntityRepository : BaseGenericRepository<EntityModel>
    {
        public EntityRepository(DbConnection connection) : base(connection)
        {
        }

        public IEnumerable<DependencyItemModel> GetDependencies(Guid id)
        {
            return _connection.Query<DependencyItemModel>($@"SELECT * FROM [core_entity_dependency] 
WHERE [EntityId] = @EntityId AND [EntityType] = @EntityType",
new { EntityId = id, EntityType = EntityType.Entity },
transaction: _transaction);
        }

        public IEnumerable<DependencyItemModel> GetDependencies(Guid id, EntityType targetEntityType)
        {
            return _connection.Query<DependencyItemModel>($@"SELECT * FROM [core_entity_dependency] 
WHERE [EntityId] = @EntityId AND [EntityType] = @EntityType AND [TargetEntityType] = @TargetEntityType",
new { EntityId = id, EntityType = EntityType.Entity, TargetEntityType = targetEntityType },
transaction: _transaction);
        }

        public void RemoveDependencies(Guid id)
        {
            _connection.Execute($@"DELETE FROM [core_entity_dependency] 
WHERE [EntityId] = @EntityId AND [EntityType] = @EntityType",
new { EntityId = id, EntityType = EntityType.Entity },
transaction: _transaction);
        }

        public void SetDependencies(Guid id, IEnumerable<DependencyItemModel> dependencies)
        {
            foreach (var dependency in dependencies)
            {
                dependency.EntityId = id;
                dependency.EntityType = EntityType.Entity;
            }
            _connection.Execute($@"DELETE FROM [core_entity_dependency] 
WHERE [EntityId] = @EntityId AND [EntityType] = @EntityType", 
new { EntityId = id, EntityType= EntityType.Entity }, 
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

        public void SetTransformations(Guid id, IEnumerable<ColumnTransformationModel> transformations)
        {
            foreach (var t in transformations)
            {
                t.TargetEntityId = id;
                t.TargetEntityType = EntityType.Entity;
            }
            _connection.Execute($@"DELETE FROM [core_entity_column_transformation] 
WHERE [TargetEntityId] = @EntityId AND [TargetEntityType] = @EntityType",
new { EntityId = id, EntityType = EntityType.Entity },
transaction: _transaction);
            _connection.Execute($@"INSERT INTO [core_entity_column_transformation](
[{nameof(ColumnTransformationModel.TargetEntityId)}],
[{nameof(ColumnTransformationModel.TargetEntityType)}],
[{nameof(ColumnTransformationModel.ColumnName)}],
[{nameof(ColumnTransformationModel.TransformerId)}])
VALUES (
@{nameof(ColumnTransformationModel.TargetEntityId)},
@{nameof(ColumnTransformationModel.TargetEntityType)},
@{nameof(ColumnTransformationModel.ColumnName)},
@{nameof(ColumnTransformationModel.TransformerId)})",
transformations,
transaction: _transaction);
        }
    }
}
